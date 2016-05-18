using Ini.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitatoBot.Executors {

    internal class FaggotStatsExecutor : IExecutionHandler {

        private static Dictionary<string, int> FaggotStats = new Dictionary<string, int>();
        private IniFile IniStorage;

        // IExecutionHandler

        public void Initialize() {
            this.IniStorage = new IniFile("faggotpoints.ini");
            LoadData();
        }

        public string GetDescription() {
            return "Use with name as argument to mark someone as a Faggot and add one faggot point. Use wihout an argument to see statistics of all faggots. Use with argument 'stats' and name of the user to see statistics of that faggot.";
        }

        public ExecutionResult CanExecute(CommandManager manager, CommandContext context) {
            if(!context.HasArguments) return ExecutionResult.Success;
            if(context.Args[0].Equals("stats") && context.Args.Length == 2) return ExecutionResult.Success;
            if(context.Args.Length == 1) return ExecutionResult.Success;
            return ExecutionResult.Denied;
        }

        public ExecutionResult Execute(CommandManager manager, CommandContext context) {
            // Print out whole statistics
            if(!context.HasArguments) {
                if(FaggotStats.Count == 0) {
                    manager.ServiceConnector.Send("There are no faggots!");
                    return ExecutionResult.Success;
                }

                manager.ServiceConnector.Send("Faggot points statistics:");
                foreach(KeyValuePair<string, int> entry in FaggotStats) {
                    manager.ServiceConnector.Send(string.Format("{0}: {1}", entry.Key, entry.Value));
                }

                return ExecutionResult.Success;
            }
            // Print out single statistic
            else if(context.Args[0].Equals("stats") && context.Args.Length == 2) {
                if(!FaggotStats.ContainsKey(context.Args[1]))
                    return ExecutionResult.Fail;

                manager.ServiceConnector.Send(string.Format("{0} has {1} faggotpoints.", context.Args[1], FaggotStats[context.Args[1]]));
                return ExecutionResult.Success;
            } 
            // Add Faggotpoint to user
            else {
                if(!FaggotStats.ContainsKey(context.Args[0])) FaggotStats.Add(context.Args[0], 0);

                FaggotStats[context.Args[0]]++;
                SaveStat(context.Args[0], FaggotStats[context.Args[0]]);
                manager.ServiceConnector.Send(string.Format("{0} is faggot!", context.Args[0]));
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
