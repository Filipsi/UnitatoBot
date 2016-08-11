using System;
using UnitatoBot.Connector.Connectors;
using UnitatoBot.Configuration;
using UnitatoBot.Command;
using UnitatoBot.Command.Execution.Executors;
using UnitatoBot.Connector;

namespace UnitatoBot {

    public class Unitato {

        static void Main(string[] args) {

            IConnector connection = new DiscordConnector(
                Config.GetEntry<string>("Email"),
                Config.GetEntry<string>("Password"),
                Config.GetEntry<ulong>("ServerUUID")
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
                .RegisterCommand("emoticon", new EmoticonExecutor())
                    .WithAlias("e")
                .RegisterCommand("burn", new BurnExecutor());
            cmdManager.Begin();

            Console.ReadKey();
        }

    }

}
