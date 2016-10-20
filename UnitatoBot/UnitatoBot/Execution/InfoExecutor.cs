using BotCore.Command;
using BotCore.Execution;
using BotCore.Util.Symbol;

namespace Unitato.Execution {

    internal class InfoExecutor : IExecutionHandler {

        // IExecutionHandler

        public string GetDescription() {
            return "Unitato will greet you and introduce itself.";
        }

        public ExecutionResult CanExecute(CommandContext context) {
            return ExecutionResult.Success;
        }

        public ExecutionResult Execute(CommandContext context) {
            context.ResponseBuilder
                .Text("Howdy")
                .Username()
                .Text(Emoticon.Greet)
                .Text("My name is")
                .Block("UnitatoBot")
                .Text("and I am cross between a Unicorn and a Potato. The most fabulous creature you could ever imagine.")
                .Text("Recently I had accident and had a surgery and I don't remember much before that, but I am sure that I wasn't purple and had wings. Oh well, Use")
                .Block("!help")
                .Text("for more.")
                .BuildAndSend();
            return ExecutionResult.Success;
        }

    }

}
