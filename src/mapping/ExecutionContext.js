const _ = require('lodash');
const chalk = require('chalk');

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

	this.log = () => console.log(
		'Executed command ' + chalk.yellow(name) +
		' with arguments ' + chalk.gray(JSON.stringify(this.args)) +
		' and mapping ' + chalk.gray(mapping.map)
	);
};
