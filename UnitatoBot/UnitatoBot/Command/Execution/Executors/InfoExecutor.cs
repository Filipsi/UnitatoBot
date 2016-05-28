using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitatoBot.Command;

namespace UnitatoBot.Command.Execution.Executors {

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
                .With("Howdy {0}! (•̀ᴗ•́)و", context.Message.Sender)
                .Space()
                .With("My name is UnitatoBot and I am cross between a Unicorn and a Potato. The most fabulous creature you could ever imagine. I am smart potato that knows how to count up to 2147483647, impressive right! I also know some dank memes and can throw a dice if you want. Oh, oh! And I was also recentry tought how to do fancy formating, thanks dev (　＾∇＾), you are da best! And I am here for {0} minutes. Use /help for more.", UptimeExecutor.GetUptime())
                .BuildAndSend();
            return ExecutionResult.Success;
        }

    }

}
