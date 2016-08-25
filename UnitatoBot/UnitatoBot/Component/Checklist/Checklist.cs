using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitatoBot.Command;
using UnitatoBot.Connector;

namespace UnitatoBot.Component.Checklist {

    internal partial class Checklist {

        public string               Id      { private set;  get; }
        public string               Owner   { private set;  get; }
        public string               Title   { private set;  get; }
        public ConnectionMessage    Message { set;          get; }

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

        public void UpdateMessage(string append = "") {
            if(Message == null)
                return;

            ResponseBuilder builder = new ResponseBuilder(Message)
                .With(SymbolFactory.Emoji.Checklist)
                .With("{0} (Checklist '{1}' by {2})", Title, Id, Owner);

            foreach(Entry entry in Entries) {
                builder
                    .NewLine()
                    .With(entry.IsChecked ? SymbolFactory.Emoji.BoxChecked : SymbolFactory.Emoji.BoxUnchecked)
                    .With(entry.Text);

                if(entry.IsChecked)
                    builder.With("(Checked by {0})", entry.CheckedBy);
            }

            if(append != null || append != string.Empty || append != "")
                builder.NewLine().With(append);

            Message.Edit(builder.Build());
        }

        public void Add(string text, bool update = true) {
            Entries.Add(new Entry(text));

            if(update)
                UpdateMessage();

            Save();
        }

        public void Remove(byte index, bool update = true) {
            Entries.RemoveAt(index);

            if(update)
                UpdateMessage();

            Save();
        }

        public bool SetEntryState(byte index, bool state, string owner) {
            if(index < Entries.Count) {
                Entries[index].SetState(state, owner);
                Save();
                return true;
            }

            return false;
        }

        public static Checklist LoadFrom(CommandManager manager, FileInfo file) {
            string data = "{}";
            using(StreamReader reader = file.OpenText()) {
                data = reader.ReadToEnd();
            }
            
            Checklist checklist = JsonConvert.DeserializeObject<Checklist>(data);

            // This is dummy message created by deserialization (contains service, origin and id)
            ConnectionMessage container = checklist.Message;
            Logger.Info("Connection: {0}, Origin: {1}, Message: {2}", container.ServiceType, container.Origin, container.Id);

            IConnector[] connectors = manager.FindServiceConnectors(container.ServiceType);
            if(connectors.Length > 0) {
                foreach(IConnector connector in connectors) {
                    ConnectionMessage msg = connector.FindMessage(container.Origin, container.Id);
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

        public void Save() {
            StreamWriter writer = File.CreateText(Path.Combine("checklist", Id + ".json"));
            writer.Write(JsonConvert.SerializeObject(this, Formatting.Indented));
            writer.Close();
            writer.Dispose();
        }

        public void Delete() {
            string path = Path.Combine("checklist", Id + ".json");

            if(File.Exists(path))
                File.Delete(path);
        }

    }

}
