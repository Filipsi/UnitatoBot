const _ = require('lodash')

module.exports = function (services) {
  // Public Interface
  this.register = (tree) => mappingTrees.push(tree)

  // Internals
  const mappingTrees = []

  const onMessageReceived = (message) => {
    if (isTrigger(message.content)) {
      // Iterate over every mapping tree with current message
      _.forEach(mappingTrees, (tree) => tree.tryExecute(message))
      // At this point message should no longer be needed
      message.dispose()
    }
  }

  const isTrigger = (text) => {
    return text.charAt(0) === '!'
  }

  // Init
  if (!_.isArray(services) || _.isEmpty(services)) {
    throw new Error('Invalid services input')
  }

  _.forEach(services, (service) => {
    if (service.onMessageReceived === undefined) {
      throw new Error('Invalid service, onMessageReceived is not defined!')
    }

    service.onMessageReceived.bind(onMessageReceived)
  })
}
