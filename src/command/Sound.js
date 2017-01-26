const fs = require('fs');
const Path = require('path');
const MappingTree = require(Path.resolve(__dirname, '../mapping/MappingTree.js'));
const Util = require(Path.resolve(__dirname, '../utilities/Util.js'));

module.exports = new MappingTree(['sound', 's'])
  .branch('list', (context, format) => {
    const audio = context.message.internals.service.getAudioInterface();

    if (audio === undefined) {
      console.log('Can not perform command on service that has no Audio Interface :(');
      return;
    }

    fs.readdir(audio.path, (err, files) => {
      if (err !== null) {
        return;
      }

      let blob = ''; let row = 0;
      files.forEach((file) => {
        file = file.replace('.mp3', '');
        if (row < 2) {
          blob += file + Util.spacer(file, 15);
          row++;
        } else {
          blob += file + '\n';
          row = 0;
        }
      });

      context.message.reply(
        format.asBlock(context.message.author) +
        ' looked through our jukebox and found these sound effects:' +
        format.asMultiline(blob)
      );
    });
  })
  .branch('[name] (channel)', (context, format) => {
    const audio = context.message.internals.service.getAudioInterface();

    if (audio === undefined) {
      console.log('Can not perform command on service that has no Audio Interface :(');
      return;
    }

    if (audio.play(context.message, context.args.name, context.args.channel)) {
      context.message.reply(
        ':musical_note: ' +
        format.asBlock(context.message.author) + ' is playing ' +
        format.asBold(context.args.name + '.mp3') + '!'
      );
    }
  });
