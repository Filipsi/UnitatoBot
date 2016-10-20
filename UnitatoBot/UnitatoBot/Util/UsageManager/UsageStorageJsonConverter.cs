using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using static Unitato.Util.UsageManager.UsageManager;

namespace Unitato.Util.UsageManager {

    internal class UsageStorageJsonConverter : JsonConverter {

        public override bool CanConvert(Type objectType) {
            return objectType.Equals(typeof(Dictionary<string, Usage>));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
            Dictionary<string, Usage> storage = new Dictionary<string, Usage>();

            JObject jObject = JObject.Load(reader);
            foreach(JProperty entry in jObject.Properties()) {
                storage.Add(entry.Name, entry.Value.ToObject<Usage>());
            }

            return storage;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
            JObject jObject = new JObject();
            foreach(KeyValuePair<string, Usage> a in (Dictionary<string, Usage>)value) {
                if(a.Value.ShouldSave)
                    jObject.Add(new JProperty(a.Key, JObject.FromObject(a.Value)));
            }

            jObject.WriteTo(writer);
        }

    }

}
