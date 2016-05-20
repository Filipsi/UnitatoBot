using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitatoBot.Command;

namespace UnitatoBot.Execution.Executors {

    internal class DiceExecutor : IExecutionHandler {

        private Random Rng;

        // IExecutionHandler

        public void Initialize() {
            this.Rng = new Random();
        }

        public string GetDescription() {
            return "d[number of sides] Rolls a n sided dice.";
        }

        public ExecutionResult CanExecute(CommandContext context) {
            return context.HasArguments && context.Args[0].ElementAt(0) == 'd' ? ExecutionResult.Success : ExecutionResult.Denied;
        }

        public ExecutionResult Execute(CommandManager manager, CommandContext context) {
            int dice;

            if(!int.TryParse(context.Args[0].Remove(0, 1), out dice)) return ExecutionResult.Fail;
            if(dice < 2) return ExecutionResult.Fail;

            context.ResponseBuilder
                .Block()
                    .Username()
                    .With("throws {0}-sided dice", dice)
                .Block()
                .Space()
                .With(Rng.Next(1, dice))
                .Build();

            return ExecutionResult.Success;
        }

    }

}
