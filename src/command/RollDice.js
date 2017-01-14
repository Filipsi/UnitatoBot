const Path = require('path')
const MappingTree = require(Path.resolve(__dirname, '../mapping/MappingTree.js'))
const Util = require(Path.resolve(__dirname, '../utilities/Util.js'))
const Format = require(Path.resolve(__dirname, '../utilities/Formatting.js'))

module.exports = new MappingTree(['roll', 'dice'])
  .branch('[sides:number]', (context) => {
    if (context.args.sides > 2) {
      context.log()
      context.message.reply(
        Format.block(context.message.author) + ' rolled ' +
        Format.bold(context.args.sides) + ' sided :game_die: that landed on number ' +
        Format.bold(Util.rand(1, context.args.sides))
      )
    }
  })
