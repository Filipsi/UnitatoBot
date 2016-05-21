using Ini.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitatoBot.Command;

namespace UnitatoBot.Command.Execution.Executors {

    internal class FaggotStatsExecutor : IExecutionHandler {

        private static Dictionary<string, int> FaggotStats = new Dictionary<string, int>();
        private IniFile IniStorage;

        // IExecutionHandler

        public void Initialize() {
            this.IniStorage = new IniFile("faggotpoints.ini");
            LoadData();
        }

        public string GetDescription() {
            return "Use with name as argument to mark someone as a Faggot and add one faggot point. Use wihout an argument to see statistics of all faggots. Use with argument 'stats' and name of the user as a second argument to see statistics of that faggot.";
        }

        public ExecutionResult CanExecute(CommandContext context) {
            if(!context.HasArguments) return ExecutionResult.Success;
            if(context.Args[0].Equals("stats") && context.Args.Length == 2) return ExecutionResult.Success;
            if(context.Args.Length == 1) return ExecutionResult.Success;
            return ExecutionResult.Denied;
        }

        public ExecutionResult Execute(CommandContext context) {
            // Print out whole statistics
            // /faggot
            if(!context.HasArguments) {
                // Build common part of response
                ResponseBuilder builder = context.ResponseBuilder
                    .Block()
                        .Username()
                        .With("requested list of faggots")
                    .Block()
                    .Space();

                // If there are no entries in stats, builds response
                if(FaggotStats.Count == 0) {
                    builder.With("(⊙.☉)7 There are no faggots.").Build();
                    return ExecutionResult.Success;
                }

                // Add each entry in stats to the response
                builder.MultilineBlock();
                foreach(KeyValuePair<string, int> entry in FaggotStats) {
                    builder.With("{0} has {1} point{2}", entry.Key, entry.Value, entry.Value > 1 ? "s" : string.Empty)
                           .NewLine();
                }
                builder.MultilineBlock();

                // Build response
                builder.Build();
                return ExecutionResult.Success;
            }
            // Print out single statistic
            // /faggot stats [name]
            else if(context.Args[0].Equals("stats") && context.Args.Length == 2) {
                // Check if there is faggot in stats with name specified by command's second argument
                if(!FaggotStats.ContainsKey(context.Args[1])) return ExecutionResult.Fail;

                // Retrives stats
                int stats = FaggotStats[context.Args[1]];

                // Build response
                context.ResponseBuilder
                    .Block()
                        .Username()
                        .With("requested faggot statistics,")
                        .With("{0} has {1} point{2}", context.Args[1], stats, stats > 1 ? "s" : string.Empty)
                    .Block()
                    .Build();
                return ExecutionResult.Success;
            } 
            // Add faggotpoint to user
            // /faggot [name]
            else {
                // If user is not in the stats, creates new entry
                if(!FaggotStats.ContainsKey(context.Args[0])) FaggotStats.Add(context.Args[0], 0);

                // Add one faggotpoint
                FaggotStats[context.Args[0]]++;

                // Retrives stats
                int stats = FaggotStats[context.Args[0]];

                // Save stat to ini file
                SaveStat(context.Args[0], stats);

                // Build response
                context.ResponseBuilder
                    .Block()
                        .Username()
                        .With("marked {0} as faggot", context.Args[0])
                    .Block()
                    .Build();

                return ExecutionResult.Success;
            }
        }

        // Helpers

        public void LoadData() {
            string users = IniStorage.ReadString("FaggotPoints", "users");
            if(users != string.Empty) {
                foreach(string user in users.Split(',')) {
                    int points = IniStorage.ReadInteger("FaggotPoints", user);
                    Console.WriteLine("Loaded faggotpoints of {0} with {1} points.", user, points);
                    FaggotStats.Add(user, points);
                }
            }
        }

        public void SaveData() {
            IniStorage.DeleteKey("FaggotPoints", "users");
            IniStorage.WriteString("FaggotPoints", "users", string.Join(",", FaggotStats.Keys));
            foreach(var user in FaggotStats) {
                Console.WriteLine("Saving {1} faggotpoints of {0}.", user.Key, user.Value);
                IniStorage.WriteInteger("FaggotPoints", user.Key, user.Value);
            }
        }

        public void SaveStat(string user, int stat) {
            string users = IniStorage.ReadString("FaggotPoints", "users");
            if(!users.Contains(user)) IniStorage.WriteString("FaggotPoints", "users", string.Join(",", FaggotStats.Keys));

            IniStorage.WriteInteger("FaggotPoints", user, stat);
        }

    }

}
