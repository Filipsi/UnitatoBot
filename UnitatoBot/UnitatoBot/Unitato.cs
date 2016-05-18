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
            DiscordConnector DiscordConnection = new DiscordConnector(Configuration.GetEntry("Email"), Configuration.GetEntry("Password"), ulong.Parse(Configuration.GetEntry("ChannelUUID")));
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

    }

}
