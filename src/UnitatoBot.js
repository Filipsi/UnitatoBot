const amp = require('app-module-path');
amp.addPath(__dirname);
amp.addPath('./../resources');

/* Modules */
const isHeroku = require('is-heroku');
const DiscordService = require('service/DiscordService');
const MappingManager = require('mapping/MappingManager');

// If we are not at Heroku, load environment
// varibales from .env file
if (!isHeroku) {
	require('dotenv').config({
		path: './.env'
	});
}

/* Core */
const discord = new DiscordService(process.env.token);
const mappingManager = new MappingManager('!', [discord]);

/* Commands */
mappingManager
  .registerModule('command/FlipCoin')
  .registerModule('command/RollDice')
  .registerModule('command/FaggotPoints')
  .registerModule('command/RussianRoulette')
  .registerModule('command/Sound');
