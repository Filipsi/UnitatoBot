module.exports = function (arg) {
  // Public Interface
  this.getName = () => name

  this.isOptional = () => optional

  this.isDataholder = () => dataholder

  this.datatype = () => datatype

  this.mach = (value) => {
    if (value === undefined) {
      return this.isOptional() ? [this.getName(), ''] : false
    }

    if (this.isDataholder()) {
      const parsed = parse(value)
      return parsed === null ? false : [this.getName(), parsed]
    }

    return value === this.getName() ? value : false
  }

  // Internals
  const isEncapsulatedWith = (encapsulator, source) => source.charAt(0) === encapsulator.charAt(0) && source.charAt(source.length - 1) === encapsulator.charAt(1)

  const parse = (value) => {
    switch (datatype) {
      case 'number':
        const num = parseInt(value)
        return isNaN(num) ? null : num
    }

    return value
  }

  // Init

  const optional = isEncapsulatedWith('()', arg)
  const dataholder = optional || isEncapsulatedWith('[]', arg)
  const unwraped = dataholder ? arg.substring(1, arg.length - 1) : arg
  const datatype = unwraped.indexOf(':') > -1 ? unwraped.substr(unwraped.indexOf(':') + 1) : undefined
  const name = datatype === undefined ? unwraped : unwraped.substring(0, unwraped.indexOf(':'))
}
