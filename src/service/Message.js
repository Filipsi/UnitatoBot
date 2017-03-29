/* Modules */
const _props = new WeakMap();

/* Class */
class Message {

	constructor (service, author, content) {
		_props.set(this, {
			service: service,
			author: author,
			content: content
		});
	}

	/* Getters */

	get author () {
		return _props.get(this).author;
	}

	get content () {
		return _props.get(this).content;
	}

	get service () {
		return _props.get(this).service;
	}

	get formatting () {
		return this.service.formatting;
	}

	/* Logic */

	reply (content, deleteOriginal) {
		this.service.reply(this, content, deleteOriginal);
	}

	edit (content) {
		_props.get(this).content = content;
		return this.service.edit(this);
	}

	delete () {
		this.service.delete(this);
	}

	dispose () {
		this.service.dispose(this);
	}

}

/* Exports */
module.exports = Message;
