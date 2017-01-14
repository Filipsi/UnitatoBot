const Path = require('path')
const MappingTree = require(Path.resolve(__dirname, '../mapping/MappingTree.js'))
const Format = require(Path.resolve(__dirname, '../utilities/Formatting.js'))

module.exports = new MappingTree(['flip', 'toss', 'coin'])
  .branch('', (context) => {
    context.log()
    context.message.reply(
      Format.block(context.message.author) + ' throwed <:silvercoin:269736398385315841> into the air, it landed on ' +
      Format.bold(Math.random() >= 0.5 ? 'heads' : 'tails')
    )
  })
