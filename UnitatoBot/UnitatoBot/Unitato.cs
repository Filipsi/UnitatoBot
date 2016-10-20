using System;
using Unitato.Execution;
using BotCore.Util;
using BotCore.Permission;
using BotCore.Bridge;
using BotCore.Command;
using BotCore.Bridge.Services;

namespace Unitato {

    public class Unitato {

        static void Main(string[] args) {
            Logger.Log("Initializing connectors");
            Logger.SectionStart();
            IService discordService = new DiscordService(Configuration.Configuration.Settings.Token);
            Logger.SectionEnd();
            Logger.Log("Connectors initialized");

            CommandManager cmdManager = new CommandManager(discordService)
                .RegisterCommand("unitato", new InfoExecutor())
                .RegisterCommand("help", new HelpExecutor())
                .RegisterCommand("invite", new InviteExecutor())
                .RegisterCommand("uptime", new UptimeExecutor())
                .RegisterCommand("roll", new DiceExecutor())
                    .WithAlias("dice")
                .RegisterCommand("praise", new PraiseExecutor())
                    .WithAlias("dan")
                .RegisterCommand("faggot", new FaggotPointsExecutor())
                .RegisterCommand("lexicon", new LexiconExecutor())
                .RegisterCommand("emoticon", new EmoticonExecutor())
                    .WithAlias("e")
                .RegisterCommand("timer", new TimerExecutor())
                .RegisterCommand("checklist", new ChecklistExecutor())
                    .WithAlias("list")
                .RegisterCommand("sound", new SoundExecutor())
                    .WithAlias("s")
                    .WithAlias("play")
                .RegisterCommand("coin", new CoinFlipExecutor())
                    .WithAlias("flip")
                .RegisterCommand("roulette", new RussianRouletteExecutor())
                    .WithAlias("rr")
                .RegisterCommand("calc", new CalcExecutor())
                .RegisterCommand("reboot", new RebootExecutor())
                .RegisterCommand("permission", new PermissionExecutor())
                    .WithAlias("perm");

            cmdManager.Begin();
            Logger.Log("Ready to go.");
            
            Console.ReadKey();
        }

    }

}
