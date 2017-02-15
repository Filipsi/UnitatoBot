const Path = require('path');
const DiscordService = require(Path.resolve(__dirname, './service/DiscordService.js'));
const MappingManager = require(Path.resolve(__dirname, './mapping/MappingManager.js'));
const Util = require(Path.resolve(__dirname, './utilities/Util.js'));
const Express = require('express');

/* Bot */
const token = Util.getDiscordToken();
const disocrdService = new DiscordService(token);
const manager = new MappingManager([disocrdService]);

// Commands
manager.register(Util.requireCommand('FlipCoin'));
manager.register(Util.requireCommand('RollDice'));
manager.register(Util.requireCommand('FaggotPoints'));
manager.register(Util.requireCommand('RussianRoulette'));
manager.register(Util.requireCommand('Sound'));

/* Web */
const web = Express();
const port = Util.getWebhostPort();

// Settings
web.set('port', port);
web.set('view engine', 'mustache');
web.set('views', __dirname + '/web');

// Engines
web.engine('mustache', require('mustache-express')());

// Routes
web.get('/', (req, res) => res.render('stats'));

// Listen
web.listen(port, () => console.log("Webpage is running on port: " + port));
