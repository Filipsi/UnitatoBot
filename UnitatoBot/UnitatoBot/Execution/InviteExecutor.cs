using BotCore.Command;
using BotCore.Execution;
using BotCore.Util.Symbol;

namespace Unitato.Execution {

    internal class InviteExecutor : IExecutionHandler {

        // IExecutionHandler

        public string GetDescription() {
            return "Prints out invitation for this bot to join given server based on service";
        }

        public bool CanExecute(CommandContext context) {
            return context.Message.ServiceType == "Discord";
        }

        public bool Execute(CommandContext context) {
            context.ResponseBuilder
                .Text("Here you go")
                .Username()
                .Text("use this so I can join you on your adventure!")
                .Text(Emoticon.Pleased)
                .NewLine()
                    .Space().Text("https://discordapp.com/oauth2/authorize?client_id={0}&scope=bot&permissions=536345655", context.Message.Service.GetServiceId())
                .Send();

            return true;
        }

    }

}
