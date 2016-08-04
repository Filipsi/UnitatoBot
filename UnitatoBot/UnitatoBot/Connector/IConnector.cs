using System;

namespace UnitatoBot.Connector {

    internal interface IConnector {

        event EventHandler<ConnectionMessageEventArgs> OnMessageReceived;

        void SendMessage(string destination, string message);

    }

}
