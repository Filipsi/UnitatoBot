using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitatoBot.Command;

namespace UnitatoBot.Command.Execution.Executors {

    internal class PraiseExecutor : IExecutionHandler, IInitializable {

        private Random Rng;

        // IInitializable

        public void Initialize() {
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
                .Block()
                    .Username()
                    .With("is praising")
                .Block()
                .With("（〜^∇^)〜")
                .Space()
                .Bold()
                    .With("Praise the {0}", context.HasArguments ? context.RawArguments : GenerateDan())
                .Bold()
                .Space()
                .With("ヽ(´▽｀)ノ")
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
