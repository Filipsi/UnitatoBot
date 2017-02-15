const Path = require('path');
const DiscordService = require(Path.resolve(__dirname, './service/DiscordService.js'));
const MappingManager = require(Path.resolve(__dirname, './mapping/MappingManager.js'));
const Util = require(Path.resolve(__dirname, './utilities/Util.js'));
const Express = require('express');
const moment = require('moment');
const start = moment();

/* Bot */
const disocrdService = new DiscordService(Util.getConfigVar('token'));
const manager = new MappingManager([disocrdService]);

// Commands
manager.register(Util.requireCommand('FlipCoin'));
manager.register(Util.requireCommand('RollDice'));
manager.register(Util.requireCommand('FaggotPoints'));
manager.register(Util.requireCommand('RussianRoulette'));
manager.register(Util.requireCommand('Sound'));

/* Web */
const web = Express();
const source = Path.join(__dirname, 'web');
const port = Util.getConfigVar('port');

// Settings
web.set('port', port);
web.set('view engine', 'mustache');
web.set('views', source);
web.use(Express.static(source)); // Serve static files from this location publicly

// Engines
web.engine('mustache', require('mustache-express')());

// Data
const stats = {
  services: manager.getServices(),
  labels: [
    {
      name: 'platform',
      value: require('is-heroku') ? 'Heroku' : 'Localhost'
    },
    {
      name: 'revision',
      value: () => Util.getPackageVar('version')
    },
    {
      name: 'uptime',
      value: moment.duration(moment().diff(start)).humanize()
    }
  ]
};

// Routes
web.get('/', (req, res) => res.render('stats', stats));

// Listen
web.listen(port, () => console.log('Webpage is running on port: ' + port));
