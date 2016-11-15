using BotCore.Execution;
using BotCore.Util.Symbol;

namespace UnitatoBot.Execution {

    internal class InfoExecutor : IExecutionHandler {

        // IExecutionHandler

        public string GetDescription() {
            return "Unitato will greet you and introduce itself.";
        }

        public bool Execute(ExecutionContext context) {
            context.ResponseBuilder
                .Text("Howdy")
                .Username()
                .Text(Emoticon.Greet)
                .Text("My name is")
                .Block("UnitatoBot")
                .Text("and I am cross between a Unicorn and a Potato. The most fabulous creature you could ever imagine.")
                .Text("I can do some cool tricks and can learn new ones if you are able to code or get in touch with my creator Filipsi#9851 with ideas. Use")
                .Block("!help")
                .Text("for more.")
                .BuildAndSend();
            return true;
        }

    }

}
