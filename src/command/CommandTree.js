const _ = require('lodash')
const Path = require('path')
const Argument = require(Path.resolve(__dirname, './Argument.js'))

module.exports = function (base) {
  // Public Interface
  this.getBase = () => base

  this.test = (message) => /!(\w+)\s*(.*)$/g.test(message.content)

  this.tryExecute = (message) => {
    // Test for valid command
    if (this.test(message)) {
      // Try to find a matching mapping
      _.forEach(mappings, (mapping) => {
        const result = this.doesMappingMach(message, mapping)

        if (result !== false) {
          // Remove arguments that are't dataholder
          let dataholders = _.remove(result, (entry) => _.isArray(entry))
          dataholders = _.fromPairs(dataholders)

          const context = {
            args: dataholders,
            message: message
          }

          mapping.executor(context)

          // Abort search after first match
          return false
        }
      })
    }
  }

  this.doesMappingMach = (message, mapping) => {
    const unwrapped = unwrap(message)

    if (unwrapped.length > mapping.args.length) {
      return false
    }

    const results = []
    let failed = false

    _.forEach(mapping.args, (arg, i) => {
      const mach = arg.mach(unwrapped[i])

      // If any argument fails, this mapping does't matching
      if (mach === false) {
        failed = true
        return false
      }

      // If argument maches, store result
      results.push(mach)
    })

    // After a sucessfull mach return results
    return failed ? false : results
  }

  this.withMapping = (mapping, executor) => {
    const args = []

    _.forEach(mapping.split(' '), (arg) => {
      args.push(new Argument(arg))
    })

    mappings.push({
      map: mapping,
      args: args,
      executor: executor
    })

    return this
  }

  // Internals
  const unwrap = (message) => {
    return /!(\w+)\s*(.*)$/g.exec(message.content)[2].split(' ')
  }

  // Init
  const mappings = []
}
