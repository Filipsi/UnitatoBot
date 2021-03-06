const MappingTree = require('mapping/MappingTree');

// Command
module.exports =
	new MappingTree(['flip', 'toss', 'coin'])
		.branch('', flip);

// Internals
function flip (context, format) {
	context.log();
	context.message.reply(
		format.asBlock(context.message.author) + ' throwed coin into the air, it landed on ' +
		format.asBold(Math.random() >= 0.5 ? 'heads' : 'tails')
	);
}
