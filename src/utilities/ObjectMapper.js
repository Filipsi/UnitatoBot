const _ = require('lodash')

module.exports = function () {
  const mapping = []
  let counter = 0

  // Public interface

  this.map = (object) => {
    counter++
    mapping[counter] = object
    return counter
  }

  this.get = (descriptor) => mapping[descriptor]

  this.dispose = (descriptor) => _.pull(mapping, mapping[descriptor])
}
