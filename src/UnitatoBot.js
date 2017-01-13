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
      context.log()

      context.message
        .reply(context.args.name + ' was set as faggot!')
        .then((message) => {
          setTimeout(() => message.edit(context.message.content + ' (edited :tada:)'), 2500)
        })
    })
    .branch('[arg1] test (op)', (context) => {
      context.log()
      context.message.delete()
    })
)
