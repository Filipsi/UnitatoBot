using System;
using BotCore.Execution;

namespace UnitatoBot.Execution {

    internal class CoinFlipExecutor : IExecutionHandler {

        private static readonly Random _rng = new Random();

        // IExecutionHandler

        public string GetDescription() {
            return "Throws coin into the air, it lands on either heads or tails.";
        }

        public bool Execute(ExecutionContext context) {
            context.ResponseBuilder
                .Username()
                .Text("throws coin into the air. It lands on")
                .Block(_rng.Next(0, 2) == 0 ? "heads" : "tails")
                .BuildAndSend();

            return true;
        }

    }

}
