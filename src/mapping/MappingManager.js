const _ = require('lodash')

module.exports = function (services) {
  // Public Interface
  this.register = (tree) => mappingTrees.push(tree)

  // Internals
  const mappingTrees = []

  const onMessageReceived = (message) => {
    if (isTrigger(message.content)) {
      _.forEach(mappingTrees, (tree) => tree.tryExecute(message))
    }
  }

  const isTrigger = (text) => {
    return text.charAt(0) === '!'
  }

  // Init
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
