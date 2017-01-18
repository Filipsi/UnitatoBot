const Path = require('path');
const Config = require('config');
const MappingTree = require(Path.resolve(__dirname, '../mapping/MappingTree.js'));
const rp = require('request-promise');

const apiKey = process.env.apiKey === undefined ? Config.get('apiKey') : process.env.apiKey;

module.exports = new MappingTree(['faggot'])
  .branch('get [user]', (context, format) => {
    get(context.args.user)
      .then((response) => {
        context.message.reply(
          format.asBlock(context.message.author) + ' looked through our records and found out that ' +
          format.asBlock(context.args.user) + ' is ' +
          (response === -1 ? 'not a faggot.' : 'faggot with ' + format.asBold(response) + ' points.')
        );
      });
  })
  .branch('mark [user]', (context, format) => {
    update(context.args.user, 1)
      .then((response) => {
        context.message.reply(
          format.asBlock(context.message.author) + ' marked ' +
          format.asBlock(context.args.user) + ' as faggot. ' +
          'Our records where updated with score of ' + format.asBold(response) + ' points.'
        );
      });
  });

function get (user) {
  const request = {
    uri: 'http://api.filipsi.net/faggotpoints/' + user,
    headers: { 'User-Agent': 'Request-Promise' },
    json: true
  };

  return rp(request);
}

function update (user, amount) {
  const request = {
    method: 'POST',
    uri: 'http://api.filipsi.net/faggotpoints/' + user,
    headers: { 'User-Agent': 'Request-Promise' },
    form: {
      key: apiKey,
      amount: amount
    },
    json: true
  };

  return rp(request);
}
