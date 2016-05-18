using Discord;
using Ini.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitatoBot.Executors;

namespace UnitatoBot {

    public class Unitato {

        static void Main(string[] args) {
            Tuple<string, string> credentials = LoadCredentials();
            DiscordConnector DiscordConnection = new DiscordConnector(credentials.Item1, credentials.Item2, 176752623406284800);

            DiscordConnection.CommandManager.RegisterCommand("unitato", new InfoExecutor());
            DiscordConnection.CommandManager.RegisterCommand("roll",    new DiceExecutor())
                                                  .WithAlias("dice");
            DiscordConnection.CommandManager.RegisterCommand("praise",  new PraiseTheDanExecutor())
                                                  .WithAlias("dan");
            DiscordConnection.CommandManager.RegisterCommand("faggot",  new FaggotStatsExecutor());
            DiscordConnection.CommandManager.RegisterCommand("help",    new HelpExecutor());

            DiscordConnection.Begin();
            Console.ReadKey();
        }

        private static Tuple<string, string> LoadCredentials() {
            IniFile iniStorage = new IniFile("credentials.ini");
            return new Tuple<string, string>(
                iniStorage.ReadString("Credentials", "username"),
                iniStorage.ReadString("Credentials", "password")
            );
        }

    }

}
