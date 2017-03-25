const Path = require('path');
const Config = require('config');
const MappingTree = require(Path.resolve(__dirname, '../mapping/MappingTree.js'));
const Util = require(Path.resolve(__dirname, '../utilities/Util.js'));
const rp = require('request-promise');
const _ = require('lodash');

const apiKey = process.env.apiKey === undefined ? Config.get('apiKey') : process.env.apiKey;

// Command
module.exports =
	new MappingTree(['faggot'])
		.branch('get [user]', get)
		.branch('list', list)
		.branch('mark [user]', update);

// Internals
function get (context, format) {
	apiGet(context.args.user)
		.then((response) => {
			context.message.reply(
				format.asBlock(context.message.author) + ' looked through our records and found out that ' +
				format.asBlock(context.args.user) + ' is ' +
				(response === -1 ? 'not a faggot.' : 'faggot with ' + format.asBold(response) + ' points.')
			);
		});
}

function list (context, format) {
	apiList()
		.then((response) => {
			let blob = '';

			_.forEach(response, (points, user) => {
				blob += points + Util.spacer(points, 4) + user + '\n';
			});

			context.message.reply(
				format.asBlock(context.message.author) +
				' looked through our storage and found these records:' +
				format.asMultiline(blob)
			);
		});
}

function update (context, format) {
	apiUpdate(context.args.user, 1)
		.then((response) => {
			context.message.reply(
				format.asBlock(context.message.author) + ' marked ' +
				format.asBlock(context.args.user) + ' as faggot. ' +
				'Our records where updated with score of ' + format.asBold(response) + ' points.'
			);
		});
}

// API
function apiGet (user) {
	const request = {
		uri: 'http://api.filipsi.net/faggotpoints/' + user,
		headers: { 'User-Agent': 'Request-Promise' },
		json: true
	};

	return rp(request);
}

function apiList () {
	const request = {
		uri: 'http://api.filipsi.net/faggotpoints',
		headers: { 'User-Agent': 'Request-Promise' },
		json: true
	};

	return rp(request);
}

function apiUpdate (user, amount) {
	const request = {
		method: 'POST',
		uri: 'http://api.filipsi.net/faggotpoints/' + user,
		headers: { 'User-Agent': 'Request-Promise' },
		form: {
			key: apiKey,
			amount: amount
		},
		json: true
	};

	return rp(request);
}
