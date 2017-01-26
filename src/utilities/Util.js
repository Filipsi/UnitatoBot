const _ = require('lodash');

module.exports = {
  rand: (min, max) => Math.floor((Math.random() * max) + min),
  spacer: (input, count) => _.repeat(' ', count - input.toString().length)
};
