const _ = require('lodash');
const MappingTree = require('mapping/MappingTree');
const Util = require('utilities/Util');

// Command
module.exports =
	new MappingTree(['rr', 'roulette'])
		.branch('', fire)
		.branch('reload [rounds:number]', reload);

// Internals
const Revolver = function (capacity) {
	const crane = [];
	let hammer = 0;
	let rounds = 0;

	this.getRounds = () => rounds;

	this.isLoaded = () => this.getRounds() > 0;

	this.getCapacity = () => capacity;

	this.reload = (loads) => {
		// Eject old rounds
		_.fill(crane, false, 0, capacity);

		// Reset hammer
		hammer = 0;

		// Load crane full
		if (rounds >= capacity) {
			_.fill(crane, true, 0, capacity);
			rounds = capacity;
		}

		// Load rounds at random :P
		while (rounds < loads) {
			const position = Util.rand(0, capacity);

			if (!crane[position]) {
				crane[position] = true;
				rounds++;
				console.log('loading pos ' + position);
				console.log('loaded: ' + rounds);
			}
		}
	};

	this.fire = () => {
		let fired = false;

		// Hammer the cylinder
		if (crane[hammer]) {
			crane[hammer] = false;
			rounds--;
			fired = true;
		}

		// Move to next cylinder
		if (hammer > capacity) {
			hammer = 0;
		} else {
			hammer++;
		}

		// Dead yet?
		return fired;
	};
};

const gun = new Revolver(6);

function fire (context, format) {
	context.log();

	const emojis = gun.fire() ? ':boom: :gun:' : format.asItalics('click') + ' :gun:';
	context.message.reply(
		emojis + '   ' + format.asBlock(context.message.author) + ' fired the gun. ' +
		(gun.isLoaded() ? format.asBold(gun.getRounds()) + ' round' + (gun.getRounds() > 1 ? 's are' : ' is') + ' left' : 'The revolver is ' + format.asBold('empty')) +
		'. Crane has ' + format.asBold(gun.getCapacity()) + ' cylinder' + (gun.getCapacity() > 1 ? 's' : '') + '.'
	);
}

function reload (context, format) {
	context.log();

	if (context.args.rounds < 1) {
		return;
	}

	gun.reload(context.args.rounds);

	context.message.reply(
		format.asBlock(context.message.author) + ' reloaded the gun. ' +
		'There ' + (gun.getRounds() > 1 ? 'are ' : 'is ') + format.asBold(gun.getRounds()) + ' round' + (gun.getRounds() > 1 ? 's' : '') +
		' in ' + format.asBold(gun.getCapacity()) + ' cylinder crane.'
	);
}
