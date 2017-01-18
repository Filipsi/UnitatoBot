const Path = require('path');
const MappingTree = require(Path.resolve(__dirname, '../mapping/MappingTree.js'));
const Util = require(Path.resolve(__dirname, '../utilities/Util.js'));

module.exports = new MappingTree(['roll', 'dice'])
  .branch('[sides:number]', (context, format) => {
    if (context.args.sides > 2) {
      context.log();
      context.message.reply(
        format.asBlock(context.message.author) + ' rolled ' +
        format.asBold(context.args.sides) + ' sided :game_die: that landed on number ' +
        format.asBold(Util.rand(1, context.args.sides))
      );
    }
  });
