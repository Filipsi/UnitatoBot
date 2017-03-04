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

    context.log();

    fs.readdir(audio.path, (err, files) => {
      if (err !== null) {
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
  })
  .branch('[name] (channel)', (context, format) => {
    const audio = context.message.internals.service.getAudioInterface();

    if (audio === undefined) {
      console.log('Can not perform command on service that has no Audio Interface :(');
      return;
    }

    context.log();

    const playinfo = audio.play(context.message, context.args.name, context.args.channel);

    if (playinfo) {
      context.message.reply(
        ':musical_note: ' +
        format.asBlock(context.message.author) + ' is playing ' +
        format.asBold(playinfo.filename) + ' on ' +
        format.asBold(playinfo.channel) + ' channel!'
      );
    }
  });
