using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitatoBot.Command;

namespace UnitatoBot.Command.Execution.Executors {

    internal class DiceExecutor : IExecutionHandler, IInitializable {

        private Random Rng;

        // IInitializable

        public void Initialize() {
            this.Rng = new Random();
        }

        // IExecutionHandler

        public string GetDescription() {
            return "d[number of sides] Rolls a n sided dice.";
        }

        public ExecutionResult CanExecute(CommandContext context) {
            return context.HasArguments && context.Args[0].ElementAt(0) == 'd' ? ExecutionResult.Success : ExecutionResult.Denied;
        }

        public ExecutionResult Execute(CommandContext context) {
            int sides;
            if(!int.TryParse(context.Args[0].Remove(0, 1), out sides)) return ExecutionResult.Fail;
            if(sides < 2) return ExecutionResult.Fail;

            context.ResponseBuilder
                .Block()
                    .Username()
                    .With("throws {0}-sided dice", sides)
                .Block()
                .Space()
                .With(Rng.Next(1, sides))
                .BuildAndSend();

            return ExecutionResult.Success;
        }

    }

}
