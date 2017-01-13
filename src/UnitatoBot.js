const Path = require('path')
const Config = require('config')
const DiscordService = require(Path.resolve(__dirname, './service/DiscordService.js'))
const MappingManager = require(Path.resolve(__dirname, './mapping/MappingManager.js'))

// Get token from process.env on heroku, if not defined fallback to config file
const token = process.env.token === undefined ? Config.get('token') : process.env.token

const disocrdService = new DiscordService(token)
const manager = new MappingManager([disocrdService])

manager.register(require(Path.resolve(__dirname, './command/RollDice.js')))
