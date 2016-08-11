using System.Collections.Generic;

namespace UnitatoBot.Command.Execution.Executors {

    internal class HelpExecutor : IExecutionHandler {

        // IExecutionHandler

        public string GetDescription() {
            return "Shows help, obviously.";
        }

        public ExecutionResult CanExecute(CommandContext context) {
            return ExecutionResult.Success;
        }

        public ExecutionResult Execute(CommandContext context) {
            ResponseBuilder builder = context.ResponseBuilder
                .With("Sure,")
                .Space()
                .Block()
                    .Username()
                .Block()
                .With("here is a list of stuff I can do: ")
                .With(SymbolFactory.Emoticon.Magic)
                .MultilineBlock();

            foreach(Command entry in context.CommandManager) {
                LinkedList<IExecutionHandler>.Enumerator enumerator = entry.GetExecutorsEnumerator();
                while(enumerator.MoveNext()) {
                    builder.With("/{0} {1}: {2}",
                                entry.Name,
                                entry.Aliases.Count > 0 ? "(" + string.Join(", ", entry.Aliases) + ")" : string.Empty,
                                enumerator.Current.GetDescription())
                           .NewLine();
                }
            }

            builder
                .MultilineBlock()
                .BuildAndSend();

            return ExecutionResult.Success;
        }
    }

}
