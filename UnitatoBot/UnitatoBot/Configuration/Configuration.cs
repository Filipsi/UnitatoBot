using Newtonsoft.Json;
using System.IO;

namespace UnitatoBot.Configuration {

    public static partial class Configuration {

        public static readonly Storage Settings;

        static Configuration() {
            Logger.SectionStart();
            Logger.Log("Loading configuration file ...");

            Logger.SectionStart();
            Settings = LoadFrom("config.json");
            CheckSanity();
            Logger.SectionEnd();

            Logger.Log("Initial configuration finished.");
            Logger.SectionEnd();
        }

        private static Storage LoadFrom(string filename) {
            Storage storage;

            if(!File.Exists(filename)) {
                storage = new Storage();
                Logger.Warn("Configuration file config.json not found, generated default");
                SaveAs(storage, "config.json");
            } else {
                using(StreamReader reader = new StreamReader(filename)) {
                    storage = JsonConvert.DeserializeObject<Storage>(reader.ReadToEnd());
                }
                Logger.Info("Configuration file config.json successfully loaded");
            }

            return storage;
        }

        private static void SaveAs(Storage storage, string filename) {
            Logger.Info("Saving configuration as {0}", filename);
            using(StreamWriter writer = new StreamWriter(filename)) {
                writer.Write(JsonConvert.SerializeObject(storage, Formatting.Indented));
            }
        }

        public static void Save() {
            SaveAs(Settings, "config.json");
        }

        private static void CheckSanity() {
            if(Settings.Token == string.Empty)
                Logger.Error("{0}#{1} is not set", typeof(Storage).FullName, "Token");
        }

    }

}
