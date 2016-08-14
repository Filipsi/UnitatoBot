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

            Message.Edit(builder.Build());
        }

        public void AddEntry(string text) {
            Entries.Add(new Entry(text));
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

        public void Save() {
            StreamWriter writer = File.CreateText(Path.Combine("checklist", Id + ".json"));
            writer.Write(JsonConvert.SerializeObject(this, Formatting.Indented));
            writer.Close();
            writer.Dispose();
        }

    }

}
