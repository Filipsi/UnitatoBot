using System;
using UnitatoBot.Connector.Connectors;
using UnitatoBot.Configuration;
using UnitatoBot.Command;
using UnitatoBot.Command.Execution.Executors;
using UnitatoBot.Connector;

namespace UnitatoBot {

    public class Unitato {

        static void Main(string[] args) {

            Logger.Log("Initializing connectors");
            Logger.SectionStart();
            IConnector connection = new DiscordConnector(Configuration.Configuration.Settings.Token);
            Logger.SectionEnd();
            Logger.Log("Connectors initialized");

            CommandManager cmdManager = new CommandManager(connection)
                .RegisterCommand("unitato", new InfoExecutor())
                .RegisterCommand("help", new HelpExecutor())
                .RegisterCommand("uptime", new UptimeExecutor())
                .RegisterCommand("roll", new DiceExecutor())
                    .WithAlias("dice")
                .RegisterCommand("praise", new PraiseExecutor())
                    .WithAlias("dan")
                .RegisterCommand("faggot", new FaggotStatsExecutor())
                .RegisterCommand("lexicon", new LexiconExecutor())
                .RegisterCommand("emoticon", new EmoticonExecutor())
                    .WithAlias("e")
                .RegisterCommand("timer", new TimerExecutor())
                .RegisterCommand("checklist", new ChecklistExecutor())
                    .WithAlias("list")
                .RegisterCommand("sound", new SoundExecutor())
                    .WithAlias("s")
                    .WithAlias("play");

            cmdManager.Begin();
            Logger.Log("Ready to go.");

            Console.ReadKey();
        }

    }

}
