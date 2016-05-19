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

namespace UnitatoBot {

    public class Unitato {

        static void Main(string[] args) {

            DiscordConnector discordConnection = new DiscordConnector(
                Config.GetEntry("Email"),
                Config.GetEntry("Password"),
                ulong.Parse(Config.GetEntry("ChannelUUID"))
            );

            discordConnection.CommandManager.RegisterCommand("unitato", new InfoExecutor());
            discordConnection.CommandManager.RegisterCommand("help",    new HelpExecutor());
            discordConnection.CommandManager.RegisterCommand("roll",    new DiceExecutor())
                                                  .WithAlias("dice");
            discordConnection.CommandManager.RegisterCommand("praise",  new PraiseTheDanExecutor())
                                                  .WithAlias("dan");
            discordConnection.CommandManager.RegisterCommand("faggot",  new FaggotStatsExecutor());

            discordConnection.Begin();
            Console.ReadKey();
        }

    }

}
