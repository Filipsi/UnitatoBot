using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Unitato.Util.UsageManager {

    internal partial class UsageManager {

        [JsonProperty]
        public int DefaultMax { private set; get; }

        [JsonProperty]
        public TimeSpan DefaultPeriod { private set; get; }

        [JsonProperty]
        [JsonConverter(typeof(UsageStorageJsonConverter))]
        private Dictionary<string, Usage> Storage = new Dictionary<string, Usage>();

        public UsageManager(int max, TimeSpan period) {
            DefaultMax = max;
            DefaultPeriod = period;
        }

        public Usage Get(string name) {
            if(!Storage.ContainsKey(name))
                Storage.Add(name, new Usage(DefaultMax, DefaultPeriod));

            return Storage[name];
        }

    }

}
