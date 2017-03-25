const path = require('path');
const DiscordService = require(path.resolve(__dirname, './service/DiscordService.js'));
const MappingManager = require(path.resolve(__dirname, './mapping/MappingManager.js'));
const Util = require(path.resolve(__dirname, './utilities/Util.js'));

// Core
const discord = new DiscordService(Util.getDiscordToken());
const mappingManager = new MappingManager('!', [discord]);

// Commands
mappingManager
  .register(Util.requireCommand('FlipCoin'))
  .register(Util.requireCommand('RollDice'))
  .register(Util.requireCommand('FaggotPoints'))
  .register(Util.requireCommand('RussianRoulette'))
  .register(Util.requireCommand('Sound'));
