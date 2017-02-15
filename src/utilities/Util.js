const Path = require('path');
const _ = require('lodash');
const isHeroku = require('is-heroku');

// Config
let Config;
function getConfigHandler () {
  if (Config === undefined || Config === null) {
    Config = require('config');
  }

  return Config;
}

// Package
let packageContent;
require('read-pkg')().then(function (pkg) {
  packageContent = pkg;
});

// Module
module.exports = {
  requireCommand: (command) => require(Path.resolve(__dirname, '../command/' + command + '.js')),
  getConfigVar: (name) => isHeroku ? process.env[name] : getConfigHandler().get(name),
  getPackageVar: (name) => packageContent === undefined ? '' : packageContent[name],
  rand: (min, max) => Math.floor((Math.random() * max) + min),
  spacer: (input, count) => _.repeat(' ', count - input.toString().length)
};
