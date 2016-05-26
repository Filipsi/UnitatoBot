using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitatoBot.Command.Execution.Executors {

    class BlameExecutor : IExecutionHandler{

        private static string DatePatten = @"d/M/yyyy hh:mm";
        private StreamWriter LogWriter;

        // IExecutionHandler

        public void Initialize() {
            // NO-OP
        }

        public string GetDescription() {
            return "Blames user specified as argument, you can use 'for' as a second argument and then specify the thing you are blaming the user for.";
        }

        public ExecutionResult CanExecute(CommandContext context) {
            return context.HasArguments ? ExecutionResult.Success : ExecutionResult.Denied;
        }

        public ExecutionResult Execute(CommandContext context) {
            // /blame [user]
            if(context.Args.Length == 1) {
                LogBlame(context.ResponseBuilder
                    .Block()
                        .Username()
                        .With("blames {0}", context.Args[0])
                    .Block()
                    .BuildAndSend());
                return ExecutionResult.Success;
            }
            // /blame [user] for [thing]
            else if(context.Args.Length > 2 && context.Args[1] == "for") {
                string blame = context.Message.Text.Replace("/" + context.Command + " " + context.Args[0] + " for ", string.Empty);
                LogBlame(context.ResponseBuilder
                    .Block()
                        .Username()
                        .With("blames {0}", context.Args[0])
                    .Block()
                    .With("I blame you for {0}", blame)
                    .BuildAndSend());
                return ExecutionResult.Success;
            }

            return ExecutionResult.Fail;
        }

        // Helpers

        private void LogBlame(string blame) {
            LogWriter = new StreamWriter("blames.txt", true, Encoding.UTF8);
            LogWriter.WriteLine(string.Format("[{0}] {1}", DateTime.Now.ToString(DatePatten), blame));
            LogWriter.Close();
        }

    }


}
