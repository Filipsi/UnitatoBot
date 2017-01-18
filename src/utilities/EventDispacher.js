const _ = require('lodash');

module.exports = function () {
  this.bind = (listener) => {
    if (this.listeners === undefined) {
      this.listeners = [listener];
      return;
    }

    if (!_.includes(this.listeners, listener)) {
      this.listeners.push(listener);
    }
  };

  this.unbind = (listener) => {
    _.pull(this.listeners, listener);
  };

  this.dispach = (args) => {
    _.forEach(this.listeners, (listener) => {
      listener(args);
    });
  };
};
