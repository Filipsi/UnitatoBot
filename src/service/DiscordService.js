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

  this.reply = (message, callback) => {
    const discriminator = message.internals.discriminator
    const promise = mapper.get(discriminator).channel.send(message.content)

    if (callback !== undefined) {
      promise.then((msg) => { callback(parse(msg)) })
    }

    mapper.dispose(discriminator)
  }

  this.edit = (message, callback) => {
    const discriminator = message.internals.discriminator
    const promise = mapper.get(discriminator).edit(message.content)

    if (callback !== undefined) {
      promise.then((msg) => { callback(message) })
    }
  }
}
