const Messsage = function (author, content) {
  // Public interface
  this.author = author

  this.content = content

  this.internals = {}

  this.dispose = () => this.internals.service.dispose(this.internals.descriptor)

  this.reply = (content, deleteOriginal) => {
    const message = new Messsage('self', content)
    message.internals = this.internals
    return this.internals.service.reply(message, deleteOriginal)
  }

  this.edit = (content) => {
    this.content = content
    return this.internals.service.edit(this)
  }

  this.delete = () => {
    return this.internals.service.delete(this)
  }

  this.toString = () => this.author + ': ' + this.content
}

module.exports = Messsage
