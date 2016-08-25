using System;
using System.IO;
using System.Text;

namespace UnitatoBot.Command.Execution.Executors {

    internal class PraiseExecutor : IExecutionHandler, IInitializable {

        public static readonly string DatePatten = @"d/M/yyyy HH:mm";

        private Random Rng;

        // IInitializable

        public void Initialize(CommandManager manager) {
            Rng = new Random();
        }

        // IExecutionHandler

        public string GetDescription() {
            return "Will praise anything specified as an argument. Even Dan, because we were praising Dan all along, even when it was prohibited.";
        }

        public ExecutionResult CanExecute(CommandContext context) {
            return ExecutionResult.Success;
        }

        public ExecutionResult Execute(CommandContext context) {
            Log(context.ResponseBuilder
                .Block()
                    .Username()
                    .With("is praising")
                .Block()
                .With(SymbolFactory.Emoticon.Dance)
                .Space()
                .Bold()
                    .With("Praise the {0}", context.HasArguments ? context.RawArguments : GenerateDan())
                .Bold()
                .Space()
                .With(SymbolFactory.Emoticon.Praise)
                .BuildAndSend());
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

        private void Log(string blame) {
            StreamWriter logger = new StreamWriter("praising.txt", true, Encoding.UTF8);
            logger.WriteLine(string.Format("[{0}] {1}", DateTime.Now.ToString(DatePatten), blame));
            logger.Close();
        }

    }

}
