using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitatoBot.Command;

namespace UnitatoBot.Execution.Executors {

    internal class PraiseTheDanExecutor : IExecutionHandler {

        private Random Rng;

        // IExecutionHandler

        public void Initialize() {
            Rng = new Random();
        }

        public string GetDescription() {
            return "Will praise the DaN!";
        }

        public ExecutionResult CanExecute(CommandContext context) {
            return ExecutionResult.Success;
        }

        public ExecutionResult Execute(CommandManager manager, CommandContext context) {
            manager.ServiceConnector.Send("**Praise the " + GenerateDan() + "**");
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
