const fs = require('fs');
const _ = require('lodash');
const path = require('path');
const chalk = require('chalk');
const Discord = require('discord.js');
const EventDispacher = require('utilities/EventDispacher');
const Message = require('service/Message');

module.exports = function (token) {
	const mapper = new WeakMap();
	const client = new Discord.Client();

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
			source.author.username,
			source.content
		);

		mapper.set(message, source);

		message.internals = {
			service: this,
			author: source.author.id
		};

		return message;
	};

	const format = {
		asBlock: (text) => '`' + text + '`',
		asItalics: (text) => '*' + text + '*',
		asBold: (text) => '**' + text + '**',
		asMultiline: (text) => '```' + text + '```'
	};

	/* Audio specific */

	const getAudioChannel = (name) => client.channels.find((channel) => channel.type === 'voice' && channel.name === name);

	const getFirstAudioChannel = (message) => message.guild.channels.find((channel) => channel.type === 'voice');

	const audioInterface = {
		source: path.resolve(__dirname, '../../resources/sounds'),
		play: function (message, filename, channel) {
			const msg = mapper.get(message);
			let filePath = path.join(this.source, filename + '.mp3');

			const search = filename.substr(0, 1) === '*';

			if (search || !fs.existsSync(filePath)) {
				// File does not exist, maybe the filename is partial
				const mp3s = fs // Buffer this maybe?
					.readdirSync(this.source)
					.filter((file) => file.indexOf('.mp3') !== -1);

				const partialCorrected = _.find(mp3s, (mp3) => {
					if (search) {
						return mp3.indexOf(filename.substr(1)) !== -1;
					}

					return mp3.substr(0, filename.length) === filename;
				});

				if (!partialCorrected) {
					return null; // File does not exist
				}

				filename = partialCorrected.replace('.mp3', '');
				filePath = path.join(this.source, partialCorrected);
			}

			if (client.voiceConnections.first() !== undefined) {
				return null; // Already playing something on this channel
			}

			// If there is not specified channel, choose one that user is connected to
			if (channel === undefined) {
				channel = msg.member.voiceChannel;

				// If user is not connected to any channel, use default
				if (channel === undefined) {
					channel = getFirstAudioChannel(msg);
				}

				// else get VoiceChannel object from it's name
			} else {
				channel = getAudioChannel(channel);

				if (channel === null) {
					return null; // There is no channel like that
				}
			}

			channel
				.join()
				.then((connection) => {
					try {
						connection
							.playFile(filePath)
							.on('end', () => connection.disconnect())
							.on('error', () => connection.disconnect());
					} catch (err) {
						channel.leave();
					}
				});

			return {
				filename: filename,
				channel: channel
			};
		}
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
	this.getAudioInterface = () => audioInterface;
};
