using System;

namespace BotCore.Bridge {

    public interface IService {

        string GetServiceType();

        string GetServiceId();

        event EventHandler<ServiceMessageEventArgs> OnMessageReceived;

        ServiceMessage SendMessage(string destination, string message);

        ServiceMessage FindMessage(string destination, string id);

    }

}
