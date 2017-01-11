const Messsage = function (author, content) {
  // Public interface
  this.author = author

  this.content = content

  this.internals = {}

  this.dispose = () => this.internals.service.dispose(this.internals.descriptor)

  this.reply = (content, callback) => {
    const message = new Messsage('self', content)
    message.internals = this.internals
    this.internals.service.reply(message, callback)
  }

  this.edit = (content, callback) => {
    this.content = content
    this.internals.service.edit(this, callback)
  }

  this.toString = () => this.author + ': ' + this.content
}

module.exports = Messsage
