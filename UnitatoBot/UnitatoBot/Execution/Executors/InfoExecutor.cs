using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitatoBot.Command;

namespace UnitatoBot.Execution.Executors {

    internal class InfoExecutor : IExecutionHandler {

        private DateTime StartTime;

        // IExecutionHandler

        public void Initialize() {
            this.StartTime = DateTime.Now;
        }

        public string GetDescription() {
            return "Unitato will greet you and introduce itself.";
        }

        public ExecutionResult CanExecute(CommandContext context) {
            return ExecutionResult.Success;
        }

        public ExecutionResult Execute(CommandManager manager, CommandContext context) {
            manager.ServiceConnector.Send("Howdy! My name is UnitatoBot and I am cross between a Unicorn and a Potato. The most fabulous creature you could ever imagine. I am smart potato that knows how to count up to 2147483647, impressive right! I also know some dank memes and can throw a dice if you want. Did you praised the DaN yet? And I am here for " + GetUptime() + " minutes. Use /help for more.");
            return ExecutionResult.Success;
        }

        // Helpers

        private string GetUptime() {
            var delta = DateTime.Now - StartTime;
            return Math.Ceiling(delta.TotalMinutes).ToString("n0");
        }

    }

}
