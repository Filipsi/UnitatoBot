using Ini.Net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace UnitatoBot {

    public static partial class Configuration {

        public enum ConfigSection {
            DiscordCredentials
            
        }

        private static Dictionary<string, string> ConfigurationMapping;
        private static IniFile ConfigFile;

        static Configuration() {
            ConfigFile = new IniFile("config.ini");
            ConfigurationMapping = new Dictionary<string, string>();

            LoadEntry(ConfigSection.DiscordCredentials, "Email");
            LoadEntry(ConfigSection.DiscordCredentials, "Password");
            LoadEntry(ConfigSection.DiscordCredentials, "ChannelUUID");
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
