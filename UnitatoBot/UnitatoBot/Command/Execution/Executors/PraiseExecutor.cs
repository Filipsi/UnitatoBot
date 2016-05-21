using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitatoBot.Command;

namespace UnitatoBot.Command.Execution.Executors {

    internal class PraiseExecutor : IExecutionHandler {

        private Random Rng;

        // IExecutionHandler

        public void Initialize() {
            Rng = new Random();
        }

        public string GetDescription() {
            return "Will praise the DaN! Or anything specified as an argument, but you know that it will not be as good as praising the dAn.";
        }

        public ExecutionResult CanExecute(CommandContext context) {
            return !context.HasArguments || (context.HasArguments && context.Args.Length == 1) ? ExecutionResult.Success : ExecutionResult.Denied;
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
                    .With("Praise the {0}", context.HasArguments ? context.Args[0] : GenerateDan())
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
