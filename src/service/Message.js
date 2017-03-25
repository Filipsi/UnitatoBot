module.exports = function (author, content) {
	// Public interface

	/* Message */
	this.author = author;

	this.content = content;

	/* Relays to Service */
	this.format = () => this.internals.service.getFormatting();

	this.dispose = () => this.internals.service.dispose(this);

	this.reply = (content, deleteOriginal) => this.internals.service.reply(this, content, deleteOriginal);

	this.edit = (content) => {
		this.content = content;
		return this.internals.service.edit(this);
	};

	this.delete = () => this.internals.service.delete(this);

	/* Internals */
	this.internals = {};

	this.toString = () => this.author + ': ' + this.content;
};
