using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitatoBot.Command;

namespace UnitatoBot.Command.Execution.Executors {

    internal class PraiseExecutor : IExecutionHandler {

        // IExecutionHandler

        public string GetDescription() {
            return "Will praise anything specified as an argument, except Dan.";
        }

        public ExecutionResult CanExecute(CommandContext context) {
            return !context.HasArguments || (context.HasArguments && context.Args.Length == 1) ? ExecutionResult.Success : ExecutionResult.Denied;
        }

        public ExecutionResult Execute(CommandContext context) {
            // Dan prasing restrictions
            if((!context.HasArguments && context.ExecutionName.Contains("dan")) || (context.HasArguments && context.Args[0].ToLower().Contains("dan"))) {
                context.ResponseBuilder
                    .Block()
                        .Username()
                        .With("wants to praise {0}", context.HasArguments ? context.Args[0] : "dan")
                    .Block()
                    .Bold()
                        .With("No.")
                    .Bold()
                    .Space()
                    .With("¬_¬")
                    .Space()
                    .With("Filipsi told me that I can't do that anymore.")
                    .BuildAndSend();

                return ExecutionResult.Success;
            }
            // Just some good old prasin' here
            else if(context.HasArguments) {
                context.ResponseBuilder
                    .Block()
                        .Username()
                        .With("is praising")
                    .Block()
                    .With("（〜^∇^)〜")
                    .Space()
                    .Bold()
                        .With("Praise the {0}", context.Args[0])
                    .Bold()
                    .Space()
                    .With("ヽ(´▽｀)ノ")
                    .BuildAndSend();
                return ExecutionResult.Success;
            }

            return ExecutionResult.Fail;
        }

    }

}
