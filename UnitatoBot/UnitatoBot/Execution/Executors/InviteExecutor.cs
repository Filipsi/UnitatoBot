using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitatoBot.Command;
using UnitatoBot.Symbol;

namespace UnitatoBot.Execution.Executors {

    internal class InviteExecutor : IExecutionHandler {

        // IExecutionHandler

        public string GetDescription() {
            return "Prints out invitation for this bot to join given server based on service";
        }

        public ExecutionResult CanExecute(CommandContext context) {
            return context.ServiceMessage.ServiceType == "Discord" ? ExecutionResult.Success : ExecutionResult.Denied;
        }

        public ExecutionResult Execute(CommandContext context) {
            context.ResponseBuilder
                .Text("Here you go")
                .Username()
                .Text("use this so I can join you on your adventure!")
                .Text(Emoticon.Pleased)
                .NewLine()
                    .Space().Text("https://discordapp.com/oauth2/authorize?client_id=218360758000418816&scope=bot&permissions=536345655")
                .Send();

            return ExecutionResult.Success;
        }

    }

}
