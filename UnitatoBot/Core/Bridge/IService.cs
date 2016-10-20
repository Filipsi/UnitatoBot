using System;
using System.Threading.Tasks;

namespace BotCore.Bridge {

    public interface IService {

        string GetServiceType();

        event EventHandler<ServiceMessageEventArgs> OnMessageReceived;

        ServiceMessage SendMessage(string destination, string message);

        ServiceMessage FindMessage(string destination, string id);

    }

}
