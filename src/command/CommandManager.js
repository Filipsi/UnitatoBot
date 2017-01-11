const _ = require('lodash')
const Path = require('path')
const EventDispacher = require(Path.resolve(__dirname, '../utilities/EventDispacher.js'))

module.exports = function (commandCharacter, services) {
  // Public Interface
  this.onCommandReceived = new EventDispacher()

  this.addCommand = (tree) => commandTrees.push(tree)

  // Internals
  const commandTrees = []

  const onMessageReceived = (message) => {
    if (isCommand(message.content)) {
      this.onCommandReceived.dispach(message)
      _.forEach(commandTrees, (tree) => tree.tryExecute(message))
    }
  }

  const isCommand = (text) => {
    return text.charAt(0) === commandCharacter
  }

  // Init
  if (commandCharacter === undefined || commandCharacter.length > 1) {
    console.error('Invalid commandCharacter input for command manager!')
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
