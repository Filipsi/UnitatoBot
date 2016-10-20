using BotCore.Command;
using BotCore.Execution;
using System;

namespace Unitato.Execution {

    internal class UptimeExecutor : IExecutionHandler {

        public static readonly DateTime StartTime = DateTime.Now;

        // IExecutionHandler

        public string GetDescription() {
            return "Prints out uptime of the bot";
        }

        public ExecutionResult CanExecute(CommandContext context) {
            return ExecutionResult.Success;
        }

        public ExecutionResult Execute(CommandContext context) {
            string uptime = GetUptime();
            context.ResponseBuilder
                .Username()
                .Text("I am here for")
                .Block(GetUptime())
                .BuildAndSend();
            return ExecutionResult.Success;
        }

        // Helpers

        public static string GetUptime() {
            return GetFormatedTime(DateTime.Now - StartTime);
        }

        public static string GetFormatedTime(TimeSpan time) {
            string text = "";
            if(time.Days > 0)
                text += string.Format("{0} day{1} ", time.Days, time.Days > 1 ? "s" : string.Empty);
            if(time.Hours > 0)
                text += string.Format("{0} hour{1} ", time.Hours, time.Hours > 1 ? "s" : string.Empty);
            if(time.Minutes > 0)
                text += string.Format("{0} minute{1} ", time.Minutes, time.Minutes > 1 ? "s" : string.Empty);
            if(time.Seconds > 0)
                text += string.Format("{0} second{1}", time.Seconds, time.Seconds > 1 ? "s" : string.Empty);

            return text;
        }

    }

}
