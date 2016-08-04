using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitatoBot.Command.Execution.Executors {

    internal class BurnExecutor : IExecutionHandler {

        // IExecutionHandler

        public string GetDescription() {
            return "Sends good old 'you got burned' image into the chat";
        }

        public ExecutionResult CanExecute(CommandContext context) {
            return context.HasArguments ? ExecutionResult.Denied : ExecutionResult.Success;
        }

        public ExecutionResult Execute(CommandContext context) {
            context.ResponseBuilder
                .With("Hey, ")
                .Block()
                    .Username()
                .Block()
                .With("thinks that you might want some cold water on that.")
                .With("https://dl.dropboxusercontent.com/u/35123963/Pictures%20and%20Gifs/burn.jpg")
                .BuildAndSend();

            return ExecutionResult.Success;
        }

    }

}
