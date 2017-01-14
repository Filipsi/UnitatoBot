const Path = require('path')
const Config = require('config')
const DiscordService = require(Path.resolve(__dirname, './service/DiscordService.js'))
const MappingManager = require(Path.resolve(__dirname, './mapping/MappingManager.js'))

function requireCommand (name) {
  return require(Path.resolve(__dirname, './command/' + name + '.js'))
}

// Get token from process.env on heroku, if not defined fallback to config file
const token = process.env.token === undefined ? Config.get('token') : process.env.token

const disocrdService = new DiscordService(token)
const manager = new MappingManager([disocrdService])

manager.register(requireCommand('FlipCoin'))
manager.register(requireCommand('RollDice'))
