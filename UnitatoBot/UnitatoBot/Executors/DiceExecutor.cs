using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitatoBot.Executors {

    internal class DiceExecutor : IExecutionHandler {

        private Random Rng;

        // IExecutionHandler

        public void Initialize() {
            this.Rng = new Random();
        }

        public string GetDescription() {
            return "d[number of sides] ;Rolls a n sided dice.";
        }

        public ExecutionResult CanExecute(CommandManager manager, CommandContext context) {
            return context.HasArguments && context.Args[0].ElementAt(0) == 'd' ? ExecutionResult.Success : ExecutionResult.Denied;
        }

        public ExecutionResult Execute(CommandManager manager, CommandContext context) {
            int dice;

            if(!int.TryParse(context.Args[0].Remove(0, 1), out dice)) return ExecutionResult.Fail;
            if(dice < 2) return ExecutionResult.Fail;

            manager.ServiceConnector.Send("*throws a " + dice + "-sided dice*");
            manager.ServiceConnector.Send(Rng.Next(1, dice).ToString());
            return ExecutionResult.Success;
        }

    }

}
