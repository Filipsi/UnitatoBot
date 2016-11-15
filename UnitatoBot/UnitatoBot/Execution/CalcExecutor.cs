using BotCore.Command;
using BotCore.Execution;
using BotCore.Util;
using BotCore.Util.Symbol;
using NCalc;
using System;

namespace Unitato.Execution {

    internal class CalcExecutor : IExecutionHandler {

        public string GetDescription() {
            return "Calculate mathematical expressions. Operators: ||, &&, =, ==, !=, <>, <, <=, >, >=, +, -, *, /, %, & (bitwise and), | (bitwise or), ^(bitwise xor), << (left shift), >>(right shift), !, not, -, ~ (bitwise not); Functions: <http://ncalc.codeplex.com/wikipage?title=functions&referringTitle=values>";
        }

        public bool CanExecute(CommandContext context) {
            return context.HasArguments;
        }

        public bool Execute(CommandContext context) {

            object result;
            try {
                result = new Expression(context.RawArguments).Evaluate();
            } catch(Exception e) {
                Logger.Info("Error cathced while Evaluateing expression from CalcExecutor: {0}", e.Message);
                return false;
            }

            context.ResponseBuilder
                .Text(Emoji.Bulb)
                .Text("Result of expression")
                .Block(context.RawArguments)
                .Text("is")
                .Block(result)
                .Send();

            return true;
        }

    }

}
