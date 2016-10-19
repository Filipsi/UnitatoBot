using System;
using UnitatoBot.Bridge.Services;
using UnitatoBot.Configuration;
using UnitatoBot.Command;
using UnitatoBot.Bridge;
using UnitatoBot.Execution.Executors;
using UnitatoBot.Permission;
using UnitatoBot.Util;
using Newtonsoft.Json;
using System.IO;

namespace UnitatoBot {

    public class Unitato {

        static void Main(string[] args) {
            /*
            var a = new UsageManager(5, TimeSpan.FromHours(1));
            a.Get("tut");
            a.Get("lulz").UseOnce();
            a.Get("lulz").UseOnce();
            a.Get("hue").UseOnce();
            a.Get("hue").UseOnce();
            a.Get("hue").UseOnce();
            a.Get("hue").UseOnce();
            a.Get("hue").UseOnce();
            a.Get("hue").UseOnce();

            using(StreamWriter fileWriter = File.CreateText("test.json")) {
                fileWriter.Write(JsonConvert.SerializeObject(a, Formatting.Indented));
            }

            using(StreamReader reader = new StreamReader("test.json")) {
                var b = JsonConvert.DeserializeObject<UsageManager>(reader.ReadToEnd());
                
            }
            */
            Permissions.Load();

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
