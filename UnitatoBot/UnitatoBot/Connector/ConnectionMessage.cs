using Newtonsoft.Json;
using System;

namespace UnitatoBot.Connector {

    [JsonObject(MemberSerialization.OptIn)]
    internal class ConnectionMessage {

        public IConnector   ConnectionProvider  { private set; get; }
        public string       Sender              { private set; get; }
        public string       Text                { private set; get; }

        [JsonProperty]
        public string Connection { private set; get; }

        [JsonProperty]
        public string Origin { private set; get; }

        [JsonProperty]
        public string Id { private set; get; }

        private Func<string, object> EditHandler;
        private Func<object>         DeleteHandler;

        public ConnectionMessage() {
            // NO-OP
        }

        public ConnectionMessage(IConnector connector, Discord.Message message) {
            ConnectionProvider = connector;
            Connection = ConnectionProvider.GetIdentificator();

            Origin = message.Channel.Id.ToString();
            Id = message.Id.ToString();

            if(message.User != null)
                Sender = message.User.Name;

            Text = message.Text;

            EditHandler = message.Edit;
            DeleteHandler = message.Delete;
        }

        public void Delete() {
            DeleteHandler.Invoke();
        }

        public void Edit(string newText) {
           EditHandler.Invoke(newText);
        }

    }

}
