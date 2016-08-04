using Discord;
using Ini.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitatoBot.Connector.Connectors;
using UnitatoBot.Configuration;
using UnitatoBot.Command;
using UnitatoBot.Command.Execution.Executors;
using UnitatoBot.Connector;

namespace UnitatoBot {

    public class Unitato {

        static void Main(string[] args) {

            IConnector connection = new DiscordConnector(
                Config.GetEntry("Email"),
                Config.GetEntry("Password"),
                ulong.Parse(Config.GetEntry("ServerUUID"))
            );

            CommandManager cmdManager = new CommandManager(connection)
                .RegisterCommand("unitato", new InfoExecutor())
                .RegisterCommand("help", new HelpExecutor())
                .RegisterCommand("uptime", new UptimeExecutor())
                .RegisterCommand("roll", new DiceExecutor())
                    .WithAlias("dice")
                .RegisterCommand("praise", new PraiseExecutor())
                    .WithAlias("dan")
                .RegisterCommand("blame", new BlameExecutor())
                .RegisterCommand("faggot", new FaggotStatsExecutor())
                .RegisterCommand("lexicon", new LexiconExecutor())
                .RegisterCommand("burn", new BurnExecutor());
            cmdManager.Begin();

            Console.ReadKey();
        }

    }

}
