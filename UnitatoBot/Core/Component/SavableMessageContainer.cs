using Newtonsoft.Json;
using System.IO;
using BotCore.Bridge;
using BotCore.Util;

namespace BotCore.Component {

    public abstract class SavableMessageContainer {

        public string           Id      { get;      }
        public string           Owner   { get;      }
        public ServiceMessage   Message { set; get; }

        private readonly FileInfo _file;

        protected SavableMessageContainer(string id, string owner, string savePath = "") {
            Id = id;
            Owner = owner;
            _file = new FileInfo(Path.Combine(savePath, Id + ".json"));
        }

        // Abstracts

        protected abstract void BuildMessageContent(ResponseBuilder builder);

        // Logic

        public void UpdateMessage() {
            if(Message == null)
                return;

            ResponseBuilder builder = new ResponseBuilder(Message);
            BuildMessageContent(builder);
            Message.Edit(builder.Build());
        }

        public void RerenderMessage() {
            ResponseBuilder builder = new ResponseBuilder(Message);
            BuildMessageContent(builder);

            Message.Delete();
            Message = builder.Send();
            Save();
        }

        // File

        public void Save() {
            using(StreamWriter writer = _file.CreateText())
                writer.Write(JsonConvert.SerializeObject(this, Formatting.Indented));
        }

        protected static string LoadFileContent(FileInfo file) {
            if(!file.Exists) {
                Logger.Warn("Unable to load {0}, file not found", file.Name);
                return null;
            }

            using(StreamReader reader = file.OpenText())
                return reader.ReadToEnd();
        }

        public void Delete() {
            if(_file.Exists)
                _file.Delete();
        }

    }

}
