using System;

namespace UnitatoBot.Connector {

    internal class ConnectionMessageEventArgs : EventArgs {

        public ConnectionMessage Message { private set; get; }

        public ConnectionMessageEventArgs(ConnectionMessage msg) {
            this.Message = msg;
        }

    }

}
