using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitatoBot.Command;
using UnitatoBot.Bridge;
using UnitatoBot.Symbol;
using UnitatoBot.Util;

namespace UnitatoBot.Component.Checklist {

    internal partial class Checklist {

        public string               Id      { private set;  get; }
        public string               Owner   { private set;  get; }
        public string               Title   { private set;  get; }
        public ServiceMessage       Message { set;          get; }
        public string               Status  { set;          get; }

        public bool IsCompleted {
            get {
                return Entries.Count > 0 && Entries.Count(e => e.IsChecked) == Entries.Count;
            }
        }

        [JsonProperty]
        private List<Entry> Entries;

        public Checklist(string id, string owner, string title) {
            Id = id;
            Owner = owner;
            Title = title;
            Entries = new List<Entry>();
        }

        public void UpdateMessage() {
            if(Message == null)
                return;

            Message.Edit(BuildMessageContent(new ResponseBuilder(Message)).Build());
        }

        private ResponseBuilder BuildMessageContent(ResponseBuilder builder) {
            builder
                .Text("Use").Block("!checklist check(or uncheck) [index](you can use multiple indexes)").Text("to check/uncheck item from the list (index from 0) (example: '!checklist check 2')")
                .NewLine()
                    .Text("Use").Block("!checklist finish").Text("after you stop using the checklist to make it not editable")
                .NewLine()
                .NewLine()
                    .Text(Emoji.Checklist)
                    .Text("{0} (Checklist", Title)
                    .Block(Id)
                    .Text("by")
                    .Block(Owner)
                    .Text(")");

            foreach(Entry entry in Entries) {
                builder
                    .NewLine()
                    .Text(entry.IsChecked ? Emoji.BoxChecked : Emoji.BoxUnchecked)
                    .Text(entry.Text);

                if(entry.IsChecked)
                    builder.Text("(Checked by {0})", entry.CheckedBy);
            }

            if(Status != null)
                builder.NewLine().Text(Status);

            return builder;
        }

        public void Add(string text, bool update = true) {
            Entries.Add(new Entry(text));

            if(update)
                UpdateMessage();

            ToFile();
        }

        public void Remove(byte index, bool update = true) {
            Entries.RemoveAt(index);

            if(update)
                UpdateMessage();

            ToFile();
        }

        public bool SetEntryState(byte index, bool state, string owner) {
            if(index < Entries.Count) {
                Entries[index].SetState(state, owner);
                ToFile();
                return true;
            }

            return false;
        }

        public void Rerender() {
            ServiceMessage msg = BuildMessageContent(new ResponseBuilder(Message)).Send();
            Message.Delete();
            Message = msg;
        }

        public static Checklist LoadFrom(CommandManager manager, FileInfo file) {
            if(!file.Exists) {
                Logger.Warn("Unable to load checklist {0}, file not found", file.Name);
                return null;
            }

            string data = "{}";
            using(StreamReader reader = file.OpenText()) {
                data = reader.ReadToEnd();
            }
            
            Checklist checklist = JsonConvert.DeserializeObject<Checklist>(data);

            // This is dummy message created by deserialization (contains service, origin and id)
            ServiceMessage container = checklist.Message;
            Logger.Info("Connection: {0}, Origin: {1}, Message: {2}", container.ServiceType, container.Origin, container.Id);

            IService[] services = manager.FindServiceType(container.ServiceType);
            if(services.Length > 0) {
                foreach(IService service in services) {
                    ServiceMessage msg = service.FindMessage(container.Origin, container.Id);
                    if(msg != null) {
                        checklist.Message = msg;
                        return checklist;
                    }
                }

                Logger.Warn("Message {0} for service {1} not found", container.Id, container.ServiceType);
            } else
                Logger.Warn("Connector for service {0} not found", container.ServiceType);

            return null;
        }

        public void ToFile() {
            using(StreamWriter writer = File.CreateText(Path.Combine("checklist", Id + ".json"))) {
                writer.Write(JsonConvert.SerializeObject(this, Formatting.Indented));
            }
        }

        public void Delete() {
            string path = Path.Combine("checklist", Id + ".json");

            if(File.Exists(path))
                File.Delete(path);
        }

    }

}
