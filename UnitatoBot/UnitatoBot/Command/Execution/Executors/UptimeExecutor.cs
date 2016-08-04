using System;

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
                .With("I am here for " + GetUptime())
                .BuildAndSend();
            return ExecutionResult.Success;
        }

        // Helpers

        public static string GetUptime() {
            TimeSpan delta = DateTime.Now - StartTime;

            string uptime = "";
            if(delta.Days > 0)      uptime += string.Format("{0} day{1} ", delta.Days, delta.Days > 1 ? "s" : string.Empty);
            if(delta.Hours > 0)     uptime += string.Format("{0} hour{1} ", delta.Hours, delta.Hours > 1 ? "s" : string.Empty);
            if(delta.Minutes > 0)   uptime += string.Format("{0} minute{1} ", delta.Minutes, delta.Minutes > 1 ? "s" : string.Empty);

            return uptime + string.Format("{0} second{1}", delta.Seconds, delta.Seconds > 1 ? "s" : string.Empty);
        }

    }

}
