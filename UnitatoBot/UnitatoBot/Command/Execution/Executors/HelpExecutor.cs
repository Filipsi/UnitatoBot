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
                .Username()
                .With(", here is a list of stuff I can do: ")
                .With(SymbolFactory.Emoticon.Magic)
                .NewLine();

            foreach(Command entry in context.CommandManager) {

                LinkedList<IExecutionHandler>.Enumerator enumerator = entry.GetExecutorsEnumerator();
                while(enumerator.MoveNext()) {

                    // Split responce into multiple messages if responce length is greater then maximal responce length
                    if(builder.Length + enumerator.Current.GetDescription().Length + 50 >= Discord.DiscordConfig.MaxMessageSize) {
                        builder.BuildAndSend();
                        builder = new ResponseBuilder(context.SourceMessage)
                            .KeepSourceMessage();
                    }

                    builder
                        .Block()
                            .With("/{0}", entry.Name)
                        .Block()
                        .With("{0}: {1}",
                            entry.Aliases.Count > 0 ? "(alias: " + string.Join(", ", entry.Aliases) + ")" : string.Empty,
                            enumerator.Current.GetDescription())
                        .NewLine();

                }

            }

            if(builder.Length > 0)
                builder.BuildAndSend();

            return ExecutionResult.Success;
        }
    }

}
