const Path = require('path')
const Config = require('config')
const DiscordService = require(Path.resolve(__dirname, './service/DiscordService.js'))
const CommandManager = require(Path.resolve(__dirname, './command/CommandManager.js'))
const CommandTree = require(Path.resolve(__dirname, './command/CommandTree.js'))

const service = new DiscordService(Config.get('token'))
const manager = new CommandManager('!', [service])

manager.addCommand(
  new CommandTree('faggot')
    .withMapping('set [name]', (context) => {
      console.log('executed mapping "set [name]" with arguments ' + JSON.stringify(context.args))
      context.message.reply(context.args.name + ' was set as faggot!')
    })
    .withMapping('[arg1] test (op)', (context) => {
      console.log('executed mapping "[arg1] test (op)" with arguments ' + JSON.stringify(context.args))
    })
)
