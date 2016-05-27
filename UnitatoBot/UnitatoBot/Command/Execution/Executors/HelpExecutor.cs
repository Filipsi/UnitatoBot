using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitatoBot.Command;

namespace UnitatoBot.Command.Execution.Executors {

    internal class HelpExecutor : IExecutionHandler {

        // IExecutionHandler

        public void Initialize() {
            // NO-OP
        }

        public string GetDescription() {
            return "Shows help, obviously.";
        }

        public ExecutionResult CanExecute(CommandContext context) {
            return ExecutionResult.Success;
        }

        public ExecutionResult Execute(CommandContext context) {
            ResponseBuilder builder = context.ResponseBuilder
                .With("Sure,")
                .Username()
                .With("here is a list of stuff I can do: (ﾉ◕ヮ◕)ﾉ*:・ﾟ✧");

            builder.MultilineBlock();
            //TODO: Print out aliases
            foreach(Command entry in context.CommandManager) {
                LinkedList<IExecutionHandler>.Enumerator enumerator = entry.GetExecutorsEnumerator();
                while(enumerator.MoveNext()) {
                    builder.With("{0}: {1}", entry.Name, enumerator.Current.GetDescription())
                           .NewLine();
                }
            }
            builder.MultilineBlock();

            builder.BuildAndSend();
            return ExecutionResult.Success;
        }
    }

}
