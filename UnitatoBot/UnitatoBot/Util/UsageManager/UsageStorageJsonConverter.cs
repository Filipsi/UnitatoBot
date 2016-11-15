using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnitatoBot.Util.UsageManager;

namespace UnitatoBot.Util.UsageManager {

    internal class UsageStorageJsonConverter : JsonConverter {

        public override bool CanConvert(Type objectType) {
            return objectType == typeof(Dictionary<string, UsageManager.Usage>);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
            Dictionary<string, UsageManager.Usage> storage = new Dictionary<string, UsageManager.Usage>();

            JObject jObject = JObject.Load(reader);
            foreach(JProperty entry in jObject.Properties()) {
                storage.Add(entry.Name, entry.Value.ToObject<UsageManager.Usage>());
            }

            return storage;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
            JObject jObject = new JObject();
            foreach(KeyValuePair<string, UsageManager.Usage> a in (Dictionary<string, UsageManager.Usage>)value) {
                if(a.Value.ShouldSave)
                    jObject.Add(new JProperty(a.Key, JObject.FromObject(a.Value)));
            }

            jObject.WriteTo(writer);
        }

    }

}
