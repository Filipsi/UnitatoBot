const Path = require('path')
const Discord = require('discord.js')
const ObjectMapper = require(Path.resolve(__dirname, '../utilities/ObjectMapper.js'))
const EventDispacher = require(Path.resolve(__dirname, '../utilities/EventDispacher.js'))
const Message = require(Path.resolve(__dirname, './Message.js'))

module.exports = function (token) {
  const mapper = new ObjectMapper()
  const client = new Discord.Client()

  // Init
  client.on('ready', () =>
    console.log(this.getName() + ' service is ready!')
  )

  client.on('message', (message) => {
    if (message.author.id === client.user.id) {
      return
    }

    this.onMessageReceived.dispach(parse(message))
  })

  client.login(token)

  // Private
  const parse = (source) => {
    const message = new Message(
      source.author.username,
      source.content
    )

    message.internals = {
      service: this,
      discriminator: mapper.map(source),
      author: source.author.id
    }

    return message
  }

  // Public interface

  this.onMessageReceived = new EventDispacher()

  this.getName = () => {
    return 'Discord#' + client.user.username
  }

  this.reply = (message, deleteOriginal) => {
    const discriminator = message.internals.discriminator
    const discordMsg = mapper.get(discriminator)

    if (deleteOriginal === undefined || deleteOriginal === true) {
      discordMsg.delete()
    }

    mapper.dispose(discriminator)

    return new Promise((resolve, reject) => {
      discordMsg.channel.send(message.content)
        .then((msg) => resolve(parse(msg)))
        .catch((err) => reject(err))
    })
  }

  this.edit = (message) => {
    return new Promise((resolve, reject) => {
      mapper.get(message.internals.discriminator).edit(message.content)
        .then((msg) => resolve(message))
        .catch((err) => reject(err))
    })
  }

  this.delete = (message) => {
    const discriminator = message.internals.discriminator
    const discordMsg = mapper.get(discriminator)

    mapper.dispose(discriminator)

    return new Promise((resolve, reject) => {
      discordMsg.delete()
        .then((msg) => resolve())
        .catch((err) => reject(err))
    })
  }
}
