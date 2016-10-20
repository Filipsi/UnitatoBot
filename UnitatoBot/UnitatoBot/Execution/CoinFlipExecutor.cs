using BotCore.Command;
using BotCore.Execution;
using System;

namespace Unitato.Execution {

    internal class CoinFlipExecutor : IExecutionHandler {

        private static readonly Random RNG = new Random();

        // IExecutionHandler

        public string GetDescription() {
            return "Throws coin into the air, it lands on either heads or tails.";
        }

        public ExecutionResult CanExecute(CommandContext context) {
            return ExecutionResult.Success;
        }

        public ExecutionResult Execute(CommandContext context) {
            context.ResponseBuilder
                .Username()
                .Text("throws coin into the air. It lands on")
                .Block(RNG.Next(0, 2) == 0 ? "heads" : "tails")
                .BuildAndSend();

            return ExecutionResult.Success;
        }

    }

}
