const _ = require('lodash')
const Path = require('path')
const EventDispacher = require(Path.resolve(__dirname, '../utilities/EventDispacher.js'))

module.exports = function (triggerCharacter, services) {
  // Public Interface
  this.onCommandReceived = new EventDispacher()

  this.register = (tree) => mappingTrees.push(tree)

  // Internals
  const mappingTrees = []

  const onMessageReceived = (message) => {
    if (isValid(message.content)) {
      this.onCommandReceived.dispach(message)
      _.forEach(mappingTrees, (tree) => tree.tryExecute(message))
    }
  }

  const isValid = (text) => {
    return text.charAt(0) === triggerCharacter
  }

  // Init
  if (triggerCharacter === undefined || triggerCharacter.length > 1) {
    console.error('Invalid triggerCharacter input for command manager!')
    return
  }

  if (!_.isArray(services) || _.isEmpty(services)) {
    console.error('Invalid services input for command manager!')
    return
  }

  _.forEach(services, (service) => {
    if (service.onMessageReceived === undefined) {
      console.error('Invalid service passsed to command manager, onMessageReceived is not defined!')
    } else {
      service.onMessageReceived.bind(onMessageReceived)
    }
  })
}
