const _ = require('lodash');

module.exports = function (name, mapping, result, message) {
  // Internals
  const parseMachResult = () => {
    // Remove arguments that are't dataholder
    let dataholders = _.remove(result, (entry) => _.isArray(entry));
    // Parse data to key/value format
    return _.fromPairs(dataholders);
  };

  // Public interface
  this.args = parseMachResult();

  this.message = message;

  this.log = () => {
    console.log(
      'Executed command "' + name + '"' +
      ' with mapping "' + mapping.map + '"' +
      ' and arguments ' + JSON.stringify(this.args));
  };
};
