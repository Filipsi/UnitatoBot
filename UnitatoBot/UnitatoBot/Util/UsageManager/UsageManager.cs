using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using System.Linq;

namespace UnitatoBot.Util.UsageManager {

    [JsonObject(IsReference = true)]
    internal partial class UsageManager {

        [JsonIgnore]
        public string FilePath {
            private set;
            get;
        }

        [JsonProperty]
        public int MaximumUses {
            private set;
            get;
        }

        [JsonProperty]
        public TimeSpan ResetPeriod {
            private set;
            get;
        }

        [JsonProperty]
        private Dictionary<string, Usage> Storage = new Dictionary<string, Usage>();

        public UsageManager(string path, int max, TimeSpan period) {
            FilePath = path;
            MaximumUses = max;
            ResetPeriod = period;
        }

        private UsageManager(string path) {
            FilePath = path;
        }

        public Usage Get(string name) {
            if(!Storage.ContainsKey(name))
                Storage.Add(name, new Usage(this));

            return Storage[name];
        }

        public static UsageManager Load(string path, int defaultMax, TimeSpan defaultPeriod) {
            FileInfo file = new FileInfo(path);

            if(!file.Exists)
                return new UsageManager(path, defaultMax, defaultPeriod);

            UsageManager manager = new UsageManager(path);

            using(StreamReader reader = file.OpenText())
                JsonConvert.PopulateObject(reader.ReadToEnd(), manager);

            return manager;
        }

        public void Save() {
            for(int i = 0; i < Storage.Count; i++) {
                if(!Storage.Values.ElementAt(i).ShouldSave)
                    Storage.Remove(Storage.Keys.ElementAt(i));
            }

            using(StreamWriter writer = new StreamWriter(FilePath))
                writer.Write(JsonConvert.SerializeObject(this, Formatting.Indented));
        }   

    }

}
