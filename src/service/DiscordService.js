const chalk = require('chalk');
const Discord = require('discord.js');
const EventDispacher = require('utilities/EventDispacher');
const DiscordAudio = require('service/DiscordAudio');
const Message = require('service/Message');

module.exports = function (token) {
	const mapper = new WeakMap();
	const client = new Discord.Client();
	const audio = new DiscordAudio(client, mapper);

	// Init
	client.on('ready', () => {
		console.log(`Service ${chalk.cyan(this.getName())} is ready.`);
		this.onServiceReady.dispach(this);
	});

	client.on('message', (message) => {
		if (message.author.id !== client.user.id) {
			this.onMessageReceived.dispach(parse(message));
		}
	});

	client.login(token);

	// Internals

	/* Message specific */

	const parse = (source) => {
		const message = new Message(
			this,
			source.author.username,
			source.content
		);

		mapper.set(message, source);
		return message;
	};

	const format = {
		asBlock: (text) => '`' + text + '`',
		asItalics: (text) => '*' + text + '*',
		asBold: (text) => '**' + text + '**',
		asMultiline: (text) => '```' + text + '```'
	};

	// Public interface

	/* Service specific */

	this.getName = () => 'Discord#' + client.user.username;

	this.getFormatting = () => format;

	this.onServiceReady = new EventDispacher();

	/* Message specific */

	this.onMessageReceived = new EventDispacher();

	this.dispose = (message) => mapper.delete(message);

	this.reply = (originalMessage, replyContent, deleteOriginal) => {
		const discordMsg = mapper.get(originalMessage);

		if (deleteOriginal === undefined || deleteOriginal === true) {
			discordMsg.delete();
		}

		return new Promise((resolve, reject) => {
			discordMsg.channel
				.send(replyContent)
				.then((msg) => resolve(parse(msg)))
				.catch((err) => reject(err));
		});
	};

	this.edit = (message) => {
		return new Promise((resolve, reject) => {
			mapper
				.get(message)
				.edit(message.content)
				.then((msg) => resolve(message))
				.catch((err) => reject(err));
		});
	};

	this.delete = (message) => {
		return new Promise((resolve, reject) => {
			mapper
				.get(message)
				.delete()
				.then((msg) => resolve())
				.catch((err) => reject(err));
		});
	};

	/* Audio specific */
	this.getAudioInterface = () => audio;
};
