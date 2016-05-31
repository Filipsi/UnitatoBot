using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitatoBot.Connector {

    internal class ConnectionMessage {

        public string Id     { private set; get; }
        public string Sender { private set; get; }
        public string Text   { private set; get; }

        private IConnector           ConnectionProvider;
        private Func<string, object> EditHandler;
        private Func<object>         DeleteHandler;

        public ConnectionMessage(IConnector connector, Discord.Message message) {
            this.ConnectionProvider = connector;
            this.Id = message.Id.ToString();
            this.Sender = message.User.Name;
            this.Text = message.Text;

            this.EditHandler = message.Edit;
            this.DeleteHandler = message.Delete;
        }

        public void Delete() {
            this.DeleteHandler.Invoke();
        }

        public void Edit(string newText) {
            this.EditHandler.Invoke(newText);
        }

    }

}
