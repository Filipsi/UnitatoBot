using System;
using Unitato.Component.Countdown;
using BotCore.Execution;
using BotCore.Command;
using BotCore.Bridge;
using BotCore.Util.Symbol;

namespace Unitato.Execution {

    internal class TimerExecutor : IExecutionHandler {

        public static readonly string DatePattenFull = @"d/M/yyyy HH:mm:ss";
        public static readonly string DatePattenTime = @"HH:mm:ss";

        // IExecutionHandler

        public string GetDescription() {
            return "Create a timer that will count down from given time to zero in seconds. You can use multiple arguments and characters like 's' for seconds, 'm' as minutes and 'h' for hours. (ex: '/timer 5m 42s')";
        }

        public ExecutionResult CanExecute(CommandContext context) {
            return context.HasArguments ? ExecutionResult.Success : ExecutionResult.Fail;
        }

        public ExecutionResult Execute(CommandContext context) {
            int sec = 0;
            foreach(string argument in context.Args) {
                int res = GetSecondsFromInput(argument);
                if(res == -1) return ExecutionResult.Fail;

                sec += res;
            }

            TimeSpan runTime = TimeSpan.FromSeconds(sec);
            DateTime finishTime = DateTime.Now.Add(runTime);
            Countdown countdown = new Countdown(runTime);

            ServiceMessage responce = context.ResponseBuilder
                .Text(Emoji.Stopwatch)
                .Username()
                .Text("created timer that will run for")
                .Block(UptimeExecutor.GetFormatedTime(runTime))
                .Text("and will finish at")
                .Block(finishTime.ToString(DatePattenFull))
                .Send();

            countdown.OnStateChanged += (sender, args) => {
                TimeSpan remining = args.Remining;

                if(remining.TotalSeconds > 0) {
                    context.ResponseBuilder
                        .Clear()
                        .Text(args.Icon)
                        .Text("Timer created by")
                        .Username()
                        .Text("is running!")
                        .NewLine()
                        .Space(8)
                        .Text("Time left")
                        .Block(UptimeExecutor.GetFormatedTime(remining))
                        .Text("; Will finish at ")
                        .Block(finishTime.ToString(DatePattenTime))
                        .Text("; Updated")
                        .Block(DateTime.Now.ToString(DatePattenTime));
                } else {
                    context.ResponseBuilder
                        .Clear()
                        .Text(args.Icon)
                        .Text("Timer created by")
                        .Username()
                        .Text("finished.")
                        .NewLine()
                        .Space(8)
                        .Text("Time passed")
                        .Block(UptimeExecutor.GetFormatedTime(runTime))
                        .Text("; Timer finised")
                        .Block(finishTime.ToString(DatePattenFull));
                }

                responce.Edit(context.ResponseBuilder.Build());
            };

            return ExecutionResult.Success;
        }

        // Utilities

        public static int GetSecondsFromInput(string input) {
            int numeric;
            
            if(int.TryParse(input.Substring(0, input.Length - 1), out numeric)) {
                switch(input.Substring(input.Length - 1)) {
                    case "s":
                        return numeric;
                    case "m":
                        return numeric * 60;
                    case "h":
                        return numeric * 60 * 60;
                }

            }

            return -1;
        }

    }

}
