const Path = require('path')
const Config = require('config')
const DiscordService = require(Path.resolve(__dirname, './service/DiscordService.js'))
const MappingManager = require(Path.resolve(__dirname, './mapping/MappingManager.js'))
const MappingTree = require(Path.resolve(__dirname, './mapping/MappingTree.js'))

const disocrdService = new DiscordService(Config.get('token'))
const manager = new MappingManager([disocrdService])

manager.register(
  new MappingTree('faggot')
    .branch('set [name]', (context) => {
      console.log('executed mapping "set [name]" with arguments ' + JSON.stringify(context.args))
      context.message.reply(context.args.name + ' was set as faggot!')
    })
    .branch('[arg1] test (op)', (context) => {
      console.log('executed mapping "[arg1] test (op)" with arguments ' + JSON.stringify(context.args))
    })
)
