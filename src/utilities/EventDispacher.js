const _ = require('lodash');

module.exports = function () {
	const listeners = [];

	this.bind = (binding) => {
		if (!_.includes(listeners, binding)) listeners.push(binding);
	};

	this.unbind = (binding) => _.pull(listeners, binding);

	this.dispach = (args) => _.forEach(listeners, (binding) => binding(args));
};
