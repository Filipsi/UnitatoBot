const Path = require('path');
const Config = require('config');
const _ = require('lodash');
const isHeroku = require('is-heroku');

module.exports = {
  requireCommand: (command) => require(Path.resolve(__dirname, '../command/' + command + '.js')),
  getDiscordToken: () => isHeroku ? process.env.token : Config.get('token'),
  getWebhostPort: () => isHeroku ? process.env.port : Config.get('port'),
  rand: (min, max) => Math.floor((Math.random() * max) + min),
  spacer: (input, count) => _.repeat(' ', count - input.toString().length)
};
