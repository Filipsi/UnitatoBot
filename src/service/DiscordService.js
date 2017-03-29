/* Modules */
const chalk = require('chalk');
const Discord = require('discord.js');
const EventDispacher = require('utilities/EventDispacher');
const DiscordAudio = require('service/DiscordAudio');
const Message = require('service/Message');
const _props = new WeakMap();

/* Class */
class DiscordService {

	constructor (token) {
		const mapper = new WeakMap();
		const client = new Discord.Client();
		const audio = new DiscordAudio(client, mapper);

		_props.set(this, {
			mapper: mapper,
			client: client,
			audio: audio,
			format: {
				asBlock: (text) => '`' + text + '`',
				asItalics: (text) => `*${text}*`,
				asBold: (text) => `**${text}**`,
				asMultiline: (text) => '```' + text + '```'
			},
			onServiceReady: new EventDispacher(),
			onMessageReceived: new EventDispacher()
		});

		client.on('ready', () => {
			console.log(`Service ${chalk.cyan(this.name)} is ready.`);
			_props.get(this).onServiceReady.dispach(this);
		});

		client.on('message', (message) => {
			if (message.author.id !== client.user.id) {
				_props.get(this).onMessageReceived.dispach(this.parse(message));
			}
		});

		client.login(token);
	}

	parse (source) {
		const message = new Message(
			this,
			source.author.username,
			source.content
		);

		_props.get(this).mapper.set(message, source);
		return message;
	}

	/* Service specific */

	get name () {
		return 'Discord#' + _props.get(this).client.user.username;
	}

	get formatting () {
		return _props.get(this).format;
	}

	get onServiceReady () {
		return _props.get(this).onServiceReady;
	}

	/* Audio */

	get audioInterface () {
		return _props.get(this).audio;
	}

	/* Message specific */

	get onMessageReceived () {
		return _props.get(this).onMessageReceived;
	}

	dispose (message) {
		_props.get(this).mapper.delete(message);
	}

	reply (originalMessage, replyContent, deleteOriginal) {
		const discordMsg = _props.get(this).mapper.get(originalMessage);

		if (deleteOriginal === undefined || deleteOriginal === true) {
			discordMsg.delete();
		}

		return new Promise((resolve, reject) => {
			discordMsg.channel
				.send(replyContent)
				.then((msg) => resolve(this.parse(msg)))
				.catch((err) => reject(err));
		});
	}

	edit (message) {
		return new Promise((resolve, reject) => {
			_props.get(this).mapper
				.get(message)
				.edit(message.content)
				.then((msg) => resolve(message))
				.catch((err) => reject(err));
		});
	}

	delete (message) {
		return new Promise((resolve, reject) => {
			_props.get(this).mapper
				.get(message)
				.delete()
				.then((msg) => resolve())
				.catch((err) => reject(err));
		});
	}

}

/* Exports */
module.exports = DiscordService;
