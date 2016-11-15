using BotCore.Command;
using BotCore.Execution;
using BotCore.Util.Symbol;
using System;
using System.Linq;

namespace Unitato.Execution {

    internal class DiceExecutor : IExecutionHandler {

        private static readonly Random RNG = new Random();

        // IExecutionHandler

        public string GetDescription() {
            return "d[number of sides] Rolls a 'n' sided dice. Example: 'roll d20'";
        }

        public bool CanExecute(CommandContext context) {
            return context.HasArguments && context.Args[0].ElementAt(0) == 'd';
        }

        public bool Execute(CommandContext context) {
            int sides;
            if(!int.TryParse(context.Args[0].Remove(0, 1), out sides))
                return false;

            if(sides < 2)
                return false;

            context.ResponseBuilder
                .Username()
                .Text("throws")
                .Block(sides)
                .Text("sided")
                .Text(Emoji.Die)
                .Text("that lands on number")
                .Block(RNG.Next(1, sides))
                .BuildAndSend();

            return true;
        }

    }

}
