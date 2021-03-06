const fs = require('fs');
const MappingTree = require('mapping/MappingTree');
const Util = require('utilities/Util');

// Command
module.exports =
	new MappingTree(['sound', 's'])
		.branch('list', list)
		.branch('[name] (channel)', play);

// Internals
function list (context, format) {
	const audio = context.message.service.audioInterface;

	if (audio === undefined) {
		console.log('Can not perform command on service that has no Audio Interface :(');
		return;
	}

	context.log();

	fs.readdir(audio.source, (err, files) => {
		if (err) {
			return;
		}

		let blob = ''; let column = 0;
		files.forEach((file) => {
			file = file.replace('.mp3', '');
			if (column < 4) {
				blob += file + Util.spacer(file, 15);
				column++;
			} else {
				blob += file + '\n';
				column = 0;
			}
		});

		context.message.reply(
			format.asBlock(context.message.author) +
			' looked through our jukebox and found these sound effects:' +
			format.asMultiline(blob)
		);
	});
}

function play (context, format) {
	const audio = context.message.service.audioInterface;

	if (audio === undefined) {
		console.log('Can not perform command on service that has no Audio Interface :(');
		return;
	}

	const playinfo = audio.play(context.message, context.args.name, context.args.channel);

	if (playinfo) {
		context.log();

		context.message.reply(
			':musical_note: ' +
			format.asBlock(context.message.author) + ' is playing ' +
			format.asBold(playinfo.filename) + ' on ' +
			format.asBold(playinfo.channel) + ' channel!'
		);
	}
}
