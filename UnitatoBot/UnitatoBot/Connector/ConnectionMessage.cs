using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitatoBot.Connector {

    internal class ConnectionMessage {

        private IConnector Connector;

        public string Id     { private set; get; }
        public string Sender { private set; get; }
        public string Text   { private set; get; }

        public ConnectionMessage(IConnector connector, Discord.Message msg) {
            this.Connector = connector;
            this.Id = msg.Id.ToString();
            this.Sender = msg.User.Name;
            this.Text = msg.Text;
        }

        public void Delete() {
            Connector.DeleteMessage(this);
        }

    }

}
