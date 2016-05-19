using Discord;
using Ini.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitatoBot.Connector.Connectors;
using UnitatoBot.Execution;
using UnitatoBot.Execution.Executors;
using UnitatoBot.Configuration;
using UnitatoBot.Command;

namespace UnitatoBot {

    public class Unitato {

        static void Main(string[] args) {

            DiscordConnector discordConnection = new DiscordConnector(
                Config.GetEntry("Email"),
                Config.GetEntry("Password"),
                ulong.Parse(Config.GetEntry("ChannelUUID"))
            );

            CommandManager cmdManager = discordConnection.CommandManager;
            cmdManager.RegisterCommand("unitato", new InfoExecutor());
            cmdManager.RegisterCommand("help", new HelpExecutor());
            cmdManager.RegisterCommand("roll", new DiceExecutor()).WithAlias("dice");
            cmdManager.RegisterCommand("praise", new PraiseTheDanExecutor()).WithAlias("dan");
            cmdManager.RegisterCommand("faggot", new FaggotStatsExecutor());
            cmdManager.RegisterCommand("lexicon", new LexiconExecutor());

            discordConnection.Begin();
            Console.ReadKey();
        }

    }

}
