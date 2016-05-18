using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitatoBot {

    internal interface IConnector {

        event EventHandler<MessageEventArgs> OnMessageReceived;

        void Send(string message);

    }

}
