using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitatoBot.Connector {

    internal interface IConnector {

        //TODO: Move this from Discord event to my event
        event EventHandler<MessageEventArgs> OnMessageReceived;

        void SendMessage(string message);

        void DeleteMessage(string id);

    }

}
