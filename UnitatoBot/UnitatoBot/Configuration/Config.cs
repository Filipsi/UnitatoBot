using Ini.Net;
using System;
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

            Load();
        }

        private static void LoadEntry(ConfigSection section, string key) {
            if(!ConfigFile.KeyExists(section.ToString(), key))
                ConfigFile.WriteString(section.ToString(), key, "");

            ConfigurationMapping.Add(key, ConfigFile.ReadString(section.ToString(), key));
        }

        private static void Load() {
            LoadEntry(ConfigSection.DiscordCredentials, "Email");
            LoadEntry(ConfigSection.DiscordCredentials, "Password");
            LoadEntry(ConfigSection.DiscordCredentials, "ServerUUID");
        }

        public static void Reload() {
            ConfigurationMapping.Clear();
            Load();
        }

        public static T GetEntry<T>(string key) {
            return ConfigurationMapping.ContainsKey(key) ? (T) Convert.ChangeType(ConfigurationMapping[key], typeof(T)) : default(T);
        }

    }

}
