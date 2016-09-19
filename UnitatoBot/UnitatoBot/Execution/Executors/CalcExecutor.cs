using NCalc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitatoBot.Command;

namespace UnitatoBot.Execution.Executors {

    internal class CalcExecutor : IExecutionHandler {

        public string GetDescription() {
            return "Calculate mathematical expressions. Operators: ||, &&, =, ==, !=, <>, <, <=, >, >=, +, -, *, /, %, & (bitwise and), | (bitwise or), ^(bitwise xor), << (left shift), >>(right shift), !, not, -, ~ (bitwise not); Functions: http://ncalc.codeplex.com/wikipage?title=functions&referringTitle=values";
        }

        public ExecutionResult CanExecute(CommandContext context) {
            return context.HasArguments ? ExecutionResult.Success : ExecutionResult.Denied;
        }

        public ExecutionResult Execute(CommandContext context) {

            object result;
            try {
                result = new Expression(context.RawArguments).Evaluate();
            } catch(Exception e) {
                Logger.Info("Error cathced while Evaluateing expression from CalcExecutor: {0}", e.Message);
                return ExecutionResult.Fail;
            }

            context.ResponseBuilder
                .Text(Symbol.Emoticon.Magic)
                .Text("Result of expression")
                .Block(context.RawArguments)
                .Text("is")
                .Block(result)
                .Send();

            return ExecutionResult.Success;
        }

    }

}
