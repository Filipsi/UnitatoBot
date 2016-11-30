using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using Discord;

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
        public string       AuthorName  { private set; get; }
        public string       AuthorId    { private set; get; }
        public string       Text        { private set; get; }

        [JsonProperty]
        public string ServiceType => Service.GetServiceType();

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
            Origin = message.Channel.Id.ToString();
            Id = message.Id.ToString();
            Text = message.Content;

            if (message.Author != null) {
                AuthorName = message.Author.Username;
                AuthorId = message.Author.Id.ToString();
            }

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
