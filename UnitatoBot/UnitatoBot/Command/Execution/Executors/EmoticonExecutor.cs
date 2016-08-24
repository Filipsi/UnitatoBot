 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitatoBot.Command.Execution.Executors {

    internal class EmoticonExecutor : IExecutionHandler {

        // IExecutionHandler

        public string GetDescription() {
            return "Prints out emoticon specified as argument. To get list of emoticons, use 'list' as a argument";
        }

        public ExecutionResult CanExecute(CommandContext context) {
            return context.HasArguments && context.Args.Count() == 1 && (context.Args[0] == "list" ||  SymbolFactory.FromName(context.Args[0]) != null) ? ExecutionResult.Success : ExecutionResult.Denied;
        }

        public ExecutionResult Execute(CommandContext context) {
            if(context.Args[0] == "list") {
                ResponseBuilder builder = context.ResponseBuilder
                    .With("Sure,")
                    .Space()
                    .Block()
                        .Username()
                    .Block()
                    .With("here is a list of emoticons I know: ")
                    .NewLine();

                foreach(SymbolFactory.Emoticon emoticon in Enum.GetValues(typeof(SymbolFactory.Emoticon))) {
                    builder.With(emoticon.ToString()).With("->").Space().With(emoticon).NewLine();
                }

                builder
                    .BuildAndSend();
            } else {
                context.ResponseBuilder
                    .Block()
                        .Username()
                    .Block()
                    .With(SymbolFactory.FromName(context.Args[0]))
                    .BuildAndSend();
            }

            return ExecutionResult.Success;
        }

    }

}
