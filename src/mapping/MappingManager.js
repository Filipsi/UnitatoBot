const _ = require('lodash');
const chalk = require('chalk');

module.exports = function (triggerCharacter, services) {
	// Public Interface
	this.register = (mappingTree) => {
		if (!mappingTrees.includes(mappingTree)) {
			console.log(`Mapping tree ${chalk.yellow(mappingTree.getRoots())} was registered.`);
			mappingTrees.push(mappingTree);
		}

		return this;
	};

	// Internals
	const mappingTrees = [];

	const onMessageReceived = (message) => {
		if (isTrigger(message.content)) {
			// Iterate over every mapping tree with current message
			_.forEach(mappingTrees, (tree) => tree.tryExecute(message));
			// At this point message should no longer be needed
			setTimeout(() => message.dispose(), 2000);
		}
	};

	const onServiceReady = (service) => {
		if (service.onMessageReceived === undefined) {
			throw new Error('Invalid service, onMessageReceived is not defined!');
		}

		service.onMessageReceived.bind(onMessageReceived);
		console.log(`Mapping manager was bound to ${chalk.cyan(service.getName())} service. ${chalk.gray('(ﾉ◕ヮ◕)ﾉ*:・ﾟ✧`')}`);
	};

	const isTrigger = (text) => text.charAt(0) === triggerCharacter;

	// Init
	if (!_.isArray(services) || _.isEmpty(services)) {
		throw new Error('Invalid services input');
	}

	_.forEach(services, (service) => {
		if (service.onServiceReady === undefined) {
			throw new Error('Invalid service, onServiceReady is not defined!');
		}

		service.onServiceReady.bind(onServiceReady);
	});
};
