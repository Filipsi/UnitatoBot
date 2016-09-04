using System;
using UnitatoBot.Command;
using UnitatoBot.Symbol;

namespace UnitatoBot.Execution.Executors {

    internal class PraiseExecutor : IExecutionHandler, IInitializable {

        public static readonly string DatePatten = @"d/M/yyyy HH:mm";

        private Random Rng;

        // IInitializable

        public void Initialize(CommandManager manager) {
            Rng = new Random();
        }

        // IExecutionHandler

        public string GetDescription() {
            return "Will praise anything specified as an argument. Even Dan, because we were praising Dan all along, even when it was prohibited.";
        }

        public ExecutionResult CanExecute(CommandContext context) {
            return ExecutionResult.Success;
        }

        public ExecutionResult Execute(CommandContext context) {
            context.ResponseBuilder
                .Username()
                .Text("is praising!")
                .Space(5)
                .Text(Emoticon.Dance)
                .Text("Praise the")
                .Bold(context.HasArguments ? context.RawArguments : GenerateDan())
                .Text(Emoticon.Praise)
                .BuildAndSend();
            return ExecutionResult.Success;
        }

        // Helpers

        private string GenerateDan() {
            char[] dan = new char[] { 'd', 'a', 'n' };
            for(byte i = 0; i < dan.Length; i++) {
                if(Rng.Next(2) == 1) dan[i] = char.ToUpper(dan[i]);
            }

            return new String(dan);
        }

    }

}
