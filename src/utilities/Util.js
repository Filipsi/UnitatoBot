const Path = require('path');
const Config = require('config');
const _ = require('lodash');

module.exports = {
	requireCommand: (command) => require(Path.resolve(__dirname, '../command/' + command + '.js')),
	getDiscordToken: () => process.env.token === undefined ? Config.get('token') : process.env.token,
	rand: (min, max) => Math.floor((Math.random() * max) + min),
	spacer: (input, count) => _.repeat(' ', count - input.toString().length)
};
