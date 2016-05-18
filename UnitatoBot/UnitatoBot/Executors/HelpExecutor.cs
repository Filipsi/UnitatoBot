using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitatoBot.Executors {

    internal class HelpExecutor : IExecutionHandler {

        // IExecutionHandler

        public void Initialize() {
            // NO-OP
        }

        public string GetDescription() {
            return "Shows help, obviously.";
        }

        public ExecutionResult CanExecute(CommandManager manager, CommandContext context) {
            return ExecutionResult.Success;
        }

        public ExecutionResult Execute(CommandManager manager, CommandContext context) {
            manager.ServiceConnector.Send("Sure, here is a list of stuff I can do:");
            foreach(var entry in manager) {
                manager.ServiceConnector.Send(string.Format("{0}: {1}", entry.Key, entry.Value.GetDescription()));
            }

            return ExecutionResult.Success;
        }
    }

}
