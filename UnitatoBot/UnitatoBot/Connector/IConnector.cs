using System;
using System.Threading.Tasks;

namespace UnitatoBot.Connector {

    internal interface IConnector {

        event EventHandler<ConnectionMessageEventArgs> OnMessageReceived;

        Task<ConnectionMessage> SendMessage(string destination, string message);

    }

}
