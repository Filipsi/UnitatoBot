using BotCore.Command;
using BotCore.Execution;
using BotCore.Util.Symbol;
using System;
using System.Linq;

namespace Unitato.Execution {

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
                int count = Enum.GetValues(typeof(Emoticon)).Length;

                ResponseBuilder builder = context.ResponseBuilder
                    .Username()
                    .Text("there {0}", count > 1 ? "are" : "is")
                    .Block(count)
                    .Text("emoticons that you can use.");

                builder
                    .TableStart(20, "Name", "Emoticon");

                foreach(Emoticon emoticon in Enum.GetValues(typeof(Emoticon))) {
                    builder.TableRow(emoticon.ToString(), SymbolFactory.AsString(emoticon));
                }

                builder
                    .TableEnd()
                    .BuildAndSend();

            } else {
                context.ResponseBuilder
                    .Username()
                    .Text("emotes")
                    .Block(context.Args[0].ToLower())
                    .Text((Emoticon)SymbolFactory.FromName(context.Args[0]))
                    .BuildAndSend();

            }

            return ExecutionResult.Success;
        }

    }

}
