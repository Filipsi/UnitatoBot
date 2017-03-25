/* Modules */
const path = require('path');
const fs = require('fs');
const soundsDirectory = path.resolve(__dirname, '../../resources/sounds');
const _props = new WeakMap();

/* Class */
class DiscordAudio {

	constructor (client, mapper) {
		const sounds = fs
			.readdirSync(this.source)
			.filter((file) => file.includes('.mp3'))
			.map((file) => file.replace('.mp3', ''));

		_props.set(this, {
			client: client,
			mapper: mapper,
			sounds: sounds
		});
	}

	/* Getters */

	get source () {
		return soundsDirectory;
	}

	/* Logic */

	getChannelByName (name) {
		return _props.get(this).client.channels.find((channel) => channel.type === 'voice' && channel.name === name);
	}

	// TODO: Get rid of getting data from message
	getDefaultChannel (message) {
		return message.guild.channels.find((channel) => channel.type === 'voice');
	}

	// TODO: Get rid of getting data from message
	getWorkingChannel (message, name) {
		if (!name) {
			return message.member.voiceChannel ? message.member.voiceChannel : this.getDefaultChannel(message);
		}

		return this.getChannelByName(name);
	}

	getSoundFileFromName (name) {
		const sounds = _props.get(this).sounds;

		if (!sounds.includes(name)) {
			name = sounds.find((sound) => sound.includes(name));
		}

		return path.join(this.source, name + '.mp3');
	}

	playOnChannel (channel, filePath) {
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
	}

	// Interface

	// TODO: Get rid of getting data from message
	play (message, sound, channel) {
		const client = _props.get(this).client;

		if (client.voiceConnections.first()) {
			return null; // Already playing something
		}

		message = _props.get(this).mapper.get(message);

		sound = this.getSoundFileFromName(sound);
		if (!sound) {
			return;
		}

		channel = this.getWorkingChannel(message, channel);
		this.playOnChannel(channel, sound);

		return {
			filename: path.basename(sound, '.mp3'),
			channel: channel.name
		};
	}

}

/* Exports */
module.exports = DiscordAudio;
