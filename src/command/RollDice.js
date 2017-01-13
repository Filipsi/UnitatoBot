const Path = require('path')
const MappingTree = require(Path.resolve(__dirname, '../mapping/MappingTree.js'))
const Util = require(Path.resolve(__dirname, '../utilities/Util.js'))
const Format = require(Path.resolve(__dirname, '../utilities/Formatting.js'))

module.exports = new MappingTree('roll')
  .branch('[sides:number]', (context) => {
    context.log()
    context.message.reply(
      Format.block(context.message.author) + ' throwed ' +
      Format.bold(context.args.sides) + ' sided :game_die: into the air that landed on number ' +
      Format.bold(Util.rand(1, context.args.sides))
    )
  })
