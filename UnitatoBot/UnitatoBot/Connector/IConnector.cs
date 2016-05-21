using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitatoBot.Connector {

    internal interface IConnector {

        event EventHandler<ConnectionMessageEventArgs> OnMessageReceived;

        void SendMessage(string message);

        void DeleteMessage(ConnectionMessage message);

    }

}
