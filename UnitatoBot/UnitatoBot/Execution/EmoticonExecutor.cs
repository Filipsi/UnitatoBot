using System;
using System.Linq;
using BotCore.Bridge;
using BotCore.Execution;
using BotCore.Util.Symbol;

namespace UnitatoBot.Execution {

    internal class EmoticonExecutor : IConditionalExecutonHandler {

        // IExecutionHandler

        public string GetDescription() {
            return "Prints out emoticon specified as argument. To get list of emoticons, use 'list' as a argument";
        }

        public bool CanExecute(ExecutionContext context) {
            return context.HasArguments && context.Args.Count() == 1 && (context.Args[0] == "list" || SymbolFactory.FromName(context.Args[0]) != null);
        }

        public bool Execute(ExecutionContext context) {
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

            return true;
        }

    }

}
