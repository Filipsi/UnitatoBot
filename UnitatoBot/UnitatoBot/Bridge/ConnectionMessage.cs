using Newtonsoft.Json;
using System;

namespace UnitatoBot.Bridge {

    internal class ServiceMessageEventArgs : EventArgs {

        public ServiceMessage Message { private set; get; }

        public ServiceMessageEventArgs(ServiceMessage msg) {
            Message = msg;
        }

    }

    [JsonObject(MemberSerialization.OptIn)]
    internal class ServiceMessage {

        public IService     Service     { private set; get; }
        public string       Sender      { private set; get; }
        public string       Text        { private set; get; }

        [JsonProperty]
        public string ServiceType { private set; get; }

        [JsonProperty]
        public string Origin { private set; get; }

        [JsonProperty]
        public string Id { private set; get; }

        private Func<string, object> EditHandler;
        private Func<object>         DeleteHandler;

        public ServiceMessage() {
            // NO-OP
        }

        public ServiceMessage(IService service, Discord.Message message) {
            Service = service;
            ServiceType = Service.GetServiceType();

            Origin = message.Channel.Id.ToString();
            Id = message.Id.ToString();

            if(message.User != null)
                Sender = message.User.Name;

            Text = message.Text;

            EditHandler = message.Edit;
            DeleteHandler = message.Delete;
        }

        public void Delete() {
            DeleteHandler.Invoke();
        }

        public void Edit(string newText) {
           EditHandler.Invoke(newText);
        }

    }

}
