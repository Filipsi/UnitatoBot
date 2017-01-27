const Path = require('path');
const DiscordService = require(Path.resolve(__dirname, './service/DiscordService.js'));
const MappingManager = require(Path.resolve(__dirname, './mapping/MappingManager.js'));
const Util = require(Path.resolve(__dirname, './utilities/Util.js'));

const token = Util.getDiscordToken();
const disocrdService = new DiscordService(token);
const manager = new MappingManager([disocrdService]);

manager.register(Util.requireCommand('FlipCoin'));
manager.register(Util.requireCommand('RollDice'));
manager.register(Util.requireCommand('FaggotPoints'));
manager.register(Util.requireCommand('RussianRoulette'));
manager.register(Util.requireCommand('Sound'));
