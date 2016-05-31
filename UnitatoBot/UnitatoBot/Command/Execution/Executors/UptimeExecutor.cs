using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace UnitatoBot.Command.Execution.Executors {

    internal class UptimeExecutor : IExecutionHandler {

        public static readonly DateTime StartTime = DateTime.Now;

        // IExecutionHandler

        public string GetDescription() {
            return "Prints out uptime of the application";
        }

        public ExecutionResult CanExecute(CommandContext context) {
            return ExecutionResult.Success;
        }

        public ExecutionResult Execute(CommandContext context) {
            string uptime = GetUptime();
            context.ResponseBuilder
                .Block()
                    .Username()
                .Block()
                .Space()
                .With("I am here for {0} minute{1}", uptime, uptime == "1" ? string.Empty : "s")
                .BuildAndSend();
            return ExecutionResult.Success;
        }

        // Helpers

        public static string GetUptime() {
            var delta = DateTime.Now - StartTime;
            return Math.Ceiling(delta.TotalMinutes).ToString("n0");
        }

    }

}
