using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitatoBot.Connector {

    internal class ConnectionMessageEventArgs : EventArgs {

        public ConnectionMessage Message { private set; get; }

        public ConnectionMessageEventArgs(ConnectionMessage msg) {
            this.Message = msg;
        }

    }

}
