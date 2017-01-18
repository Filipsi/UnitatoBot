const Path = require('path');
const Discord = require('discord.js');
const EventDispacher = require(Path.resolve(__dirname, '../utilities/EventDispacher.js'));
const Message = require(Path.resolve(__dirname, './Message.js'));

module.exports = function (token) {
  const mapper = new WeakMap();
  const client = new Discord.Client();

  // Init
  client.on('ready', () =>
    console.log(this.getName() + ' service is ready!')
  );

  client.on('message', (message) => {
    if (message.author.id === client.user.id) {
      return;
    }

    this.onMessageReceived.dispach(parse(message));
  });

  client.login(token);

  // Internals
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

  // Public interface

  /* Service specific */

  this.getName = () => 'Discord#' + client.user.username;

  this.getFormatting = () => format;

  /* Message */

  this.onMessageReceived = new EventDispacher();

  this.dispose = (message) => mapper.delete(message);

  this.reply = (originalMessage, replyContent, deleteOriginal) => {
    const discordMsg = mapper.get(originalMessage);

    if (deleteOriginal === undefined || deleteOriginal === true) {
      discordMsg.delete();
    }

    return new Promise((resolve, reject) => {
      discordMsg.channel.send(replyContent)
        .then((msg) => resolve(parse(msg)))
        .catch((err) => reject(err));
    });
  };

  this.edit = (message) => {
    return new Promise((resolve, reject) => {
      mapper.get(message).edit(message.content)
        .then((msg) => resolve(message))
        .catch((err) => reject(err));
    });
  };

  this.delete = (message) => {
    return new Promise((resolve, reject) => {
      mapper.get(message).delete()
        .then((msg) => resolve())
        .catch((err) => reject(err));
    });
  };
};
