module.exports = function (arg) {
  // Public Interface
  this.getName = () => name

  this.isOptional = () => optional

  this.isDataholder = () => dataholder

  this.mach = (text) => {
    if (text === undefined) {
      return this.isOptional() ? [this.getName(), ''] : false
    }

    return this.isDataholder() ? [this.getName(), text] : text === this.getName() ? text : false
  }

  // Internals
  const isEncapsulatedWith = (encapsulator, source) => source.charAt(0) === encapsulator.charAt(0) && source.charAt(source.length - 1) === encapsulator.charAt(1)

  // Init
  const optional = isEncapsulatedWith('()', arg)
  const dataholder = optional || isEncapsulatedWith('[]', arg)
  const name = dataholder ? arg.substring(1, arg.length - 1) : arg
}
