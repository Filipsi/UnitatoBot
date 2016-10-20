using Newtonsoft.Json;
using System.IO;
using UnitatoBot.Bridge;
using UnitatoBot.Command;
using UnitatoBot.Util;

namespace UnitatoBot.Component.Common {

    internal abstract class SavableMessageContainer {

        public string           Id      { private set;  get; }
        public string           Owner   { private set;  get; }
        public string           Title   { private set;  get; }
        public ServiceMessage   Message { set;          get; }

        protected string        SavePath = string.Empty;

        public SavableMessageContainer(string id, string owner, string title) {
            Id = id;
            Owner = owner;
            Title = title;
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
            ServiceMessage msg = builder.Send();
            Message.Delete();
            Message = msg;
            Save();
        }

        // File

        public void Save() {
            using(StreamWriter writer = File.CreateText(Path.Combine(SavePath, Id + ".json"))) {
                writer.Write(JsonConvert.SerializeObject(this, Formatting.Indented));
            }
        }

        protected static string LoadFileContent(FileInfo file) {
            if(!file.Exists) {
                Logger.Warn("Unable to load {0}, file not found", file.Name);
                return null;
            }

            string data;
            using(StreamReader reader = file.OpenText()) {
                data = reader.ReadToEnd();
            }

            return data;
        }

        public void Delete() {
            FileInfo file = new FileInfo(Path.Combine(SavePath, Id + ".json"));
            if(file.Exists) file.Delete();
        }

    }

}
