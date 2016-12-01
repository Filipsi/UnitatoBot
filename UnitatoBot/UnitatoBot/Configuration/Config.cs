using System.IO;
using BotCore.Util;
using Newtonsoft.Json;

namespace UnitatoBot.Configuration {

    public static partial class Config {

        private static readonly FileInfo ConfigFile = new FileInfo("config.json");

        public  static Storage Settings { get; private set; }

        public static void Load() {
            Logger.Log("Loading configuration file ...");
            Logger.SectionStart();

            if(!ConfigFile.Exists) {
                Settings = new Storage();

                Logger.Warn("Config file config.json not found, using default!");
                Save();
            } else {
                using(StreamReader reader = ConfigFile.OpenText())
                    Settings = JsonConvert.DeserializeObject<Storage>(reader.ReadToEnd());

                Logger.Info("Config file config.json successfully loaded");
            }

            // Sanity check
            if(Settings.Token == string.Empty)
                Logger.Error("{0}#{1} is not set", typeof(Storage).FullName, "Token");
            if(Settings.ApiKey == string.Empty)
                Logger.Error("{0}#{1} is not set", typeof(Storage).FullName, "ApiKey");

            Logger.SectionEnd();
            Logger.Log("Initial configuration finished.");
        }

        public static void Save() {
            Logger.Info("Saving configuration as {0}", ConfigFile.Name);

            using(StreamWriter writer = ConfigFile.CreateText())
                writer.Write(JsonConvert.SerializeObject(Settings, Formatting.Indented));
        }

    }

}
