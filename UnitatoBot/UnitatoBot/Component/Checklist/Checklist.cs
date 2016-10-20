using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnitatoBot.Command;
using UnitatoBot.Bridge;
using UnitatoBot.Symbol;
using UnitatoBot.Util;
using UnitatoBot.Component.Common;

namespace UnitatoBot.Component.Checklist {

    internal partial class Checklist : SavableMessageContainer {

        public string Status { set; get; }

        public bool IsCompleted {
            get {
                return Entries.Count > 0 && Entries.Count(e => e.IsChecked) == Entries.Count;
            }
        }

        [JsonProperty]
        private List<Entry> Entries;

        public Checklist(string id, string owner, string title) : base(id, owner, title) {
            SavePath = "checklist";
            Entries = new List<Entry>();
        }

        // Base

        protected override void BuildMessageContent(ResponseBuilder builder) {
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
        }

        public static Checklist Load(FileInfo file, CommandManager manager) {
            Checklist checklist = JsonConvert.DeserializeObject<Checklist>(LoadFileContent(file));

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

        // Entires

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

    }

}
