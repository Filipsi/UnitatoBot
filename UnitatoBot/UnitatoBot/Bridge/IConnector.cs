using System;
using System.Threading.Tasks;

namespace UnitatoBot.Connector {

    internal interface IConnector {

        string GetServiceType();

        event EventHandler<ConnectionMessageEventArgs> OnMessageReceived;

        ConnectionMessage SendMessage(string destination, string message);

        ConnectionMessage FindMessage(string destination, string id);

    }

}
