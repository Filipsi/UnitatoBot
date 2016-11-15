using System.Linq;
using BotCore.Bridge;
using BotCore.Execution;
using BotCore.Util.Symbol;

namespace UnitatoBot.Execution {

    internal class HelpExecutor : IExecutionHandler {

        // IExecutionHandler

        public string GetDescription() {
            return "Shows help, obviously.Use ExecutionDispacher name or alias as argument to show help only for specified ExecutionDispacher.";
        }

        public bool Execute(ExecutionContext context) {
            if(!context.HasArguments) {
                PrintHelpInfo(context);
                return true;

            } else if(context.Args.Length == 1) {
                ExecutionDispacher executionDispacher = context.ExecutionManager.FindCommand(context.Args[0].ToLower());

                if(executionDispacher != null) {
                    PrintHelpFor(context, executionDispacher);
                    return true;
                }
            }

            return false;
        }

        // Logic

        private void PrintHelpFor(ExecutionContext context, ExecutionDispacher executionDispacher) {
            ResponseBuilder builder = context.ResponseBuilder
                .Username()
                .Text("here is help for ExecutionDispacher")
                .Block(executionDispacher.Name)
                .Text(Emoticon.Magic)
                .NewLine()
                .NewLine();

            BuildCommandInfo(builder, executionDispacher);

            if(builder.Length > 0)
                builder.Send();
        }

        private void PrintHelpInfo(ExecutionContext context) {
            ResponseBuilder builder = context.ResponseBuilder
                .Username()
                .Text("here is a list of stuff I can do: ")
                .Text(Emoticon.Magic)
                .NewLine()
                .NewLine();

            foreach(ExecutionDispacher entry in context.ExecutionManager) {
                BuildCommandInfo(builder, entry);
            }

            if(builder.Length > 0)
                builder.Send();
        }

        private void BuildCommandInfo(ResponseBuilder builder, ExecutionDispacher executionDispacher) {
            foreach(IExecutionHandler executor in executionDispacher) {
                // Split responce into multiple messages if responce length is greater then maximal responce length
                if(builder.Length + executor.GetDescription().Length + 50 >= 2000) {
                    builder.Send();
                    builder.Clear().KeepSourceMessage();
                }

                builder
                    .Block("!{0}", executionDispacher.Name)
                    .Text("{0}: {1}",
                        executionDispacher.Alias.Any() ? "(alias: " + string.Join(", ", executionDispacher.Alias) + ")" : string.Empty,
                        executor.GetDescription())
                    .NewLine();
            }
        }

    }

}
