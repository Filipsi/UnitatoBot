using System;
using System.Threading.Tasks;

namespace UnitatoBot.Bridge {

    internal interface IService {

        string GetServiceType();

        event EventHandler<ServiceMessageEventArgs> OnMessageReceived;

        ServiceMessage SendMessage(string destination, string message);

        ServiceMessage FindMessage(string destination, string id);

    }

}
