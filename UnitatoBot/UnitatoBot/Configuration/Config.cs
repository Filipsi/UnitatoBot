using Ini.Net;
using System.Collections.Generic;

namespace UnitatoBot.Configuration {

    public static partial class Config {

        public enum ConfigSection {
            DiscordCredentials
        }

        private static Dictionary<string, string> ConfigurationMapping;
        private static IniFile ConfigFile;

        static Config() {
            ConfigFile = new IniFile("config.ini");
            ConfigurationMapping = new Dictionary<string, string>();

            LoadEntry(ConfigSection.DiscordCredentials, "Email");
            LoadEntry(ConfigSection.DiscordCredentials, "Password");
            LoadEntry(ConfigSection.DiscordCredentials, "ServerUUID");
        }

        private static void LoadEntry(ConfigSection section, string key) {
            if(!ConfigFile.KeyExists(section.ToString(), key)) {
                ConfigFile.WriteString(section.ToString(), key, "");
            }

            ConfigurationMapping.Add(key, ConfigFile.ReadString(section.ToString(), key));
        }

        public static string GetEntry(string key) {
            return ConfigurationMapping.ContainsKey(key) ? ConfigurationMapping[key] : null;
        }

    }

}
