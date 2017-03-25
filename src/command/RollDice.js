const Path = require('path');
const MappingTree = require(Path.resolve(__dirname, '../mapping/MappingTree.js'));
const Util = require(Path.resolve(__dirname, '../utilities/Util.js'));

// Command
module.exports =
	new MappingTree(['roll', 'dice'])
		.branch('[sides:number]', roll);

// Internals
function roll (context, format) {
	if (context.args.sides < 3) {
		return;
	}

	context.log();
	context.message.reply(
		format.asBlock(context.message.author) + ' rolled ' +
		format.asBold(context.args.sides) + ' sided :game_die: that landed on number ' +
		format.asBold(Util.rand(1, context.args.sides))
	);
}
