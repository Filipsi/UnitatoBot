using Newtonsoft.Json;
using System;
using Discord.WebSocket;
using System.Threading.Tasks;
using Discord;
using Discord.Rest;

namespace BotCore.Bridge {

    public class ServiceMessageEventArgs : EventArgs {

        public ServiceMessage Message { private set; get; }

        public ServiceMessageEventArgs(ServiceMessage msg) {
            Message = msg;
        }

    }

    [JsonObject(MemberSerialization.OptIn)]
    public class ServiceMessage {

        public IService     Service     { private set; get; }
        public string       Sender      { private set; get; }
        public string       Text        { private set; get; }

        [JsonProperty]
        public string ServiceType { private set; get; }

        [JsonProperty]
        public string Origin { private set; get; }

        [JsonProperty]
        public string Id { private set; get; }

        private readonly Func<string, Task> _editHandler;
        private readonly Func<Task>         _deleteHandler;

        public ServiceMessage() {
            // NO-OP
        }

        public ServiceMessage(IService service, IUserMessage message) {
            Service = service;
            ServiceType = Service.GetServiceType();

            Origin = message.Channel.Id.ToString();
            Id = message.Id.ToString();

            if (message.Author != null)
                Sender = message.Author.Username;

            Text = message.Content;

            _editHandler = async (content) => { await message.ModifyAsync((c) => { c.Content = content; }); };
            _deleteHandler = async () => { await message.DeleteAsync(); };
        }

        public void Delete() {
            _deleteHandler.Invoke();
        }

        public void Edit(string newText) {
           _editHandler.Invoke(newText);
        }

    }

}
