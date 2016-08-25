namespace UnitatoBot.Command.Execution.Executors {

    internal class InfoExecutor : IExecutionHandler {

        // IExecutionHandler

        public string GetDescription() {
            return "Unitato will greet you and introduce itself.";
        }

        public ExecutionResult CanExecute(CommandContext context) {
            return ExecutionResult.Success;
        }

        public ExecutionResult Execute(CommandContext context) {
            context.ResponseBuilder
                .With("Howdy {0}!", context.SourceMessage.Sender)
                .With(SymbolFactory.Emoticon.Greet)
                .Space()
                .With("My name is UnitatoBot and I am cross between a Unicorn and a Potato. The most fabulous creature you could ever imagine. I am smart potato that knows how to count up to 2147483647, impressive right! I also know some dank memes and can throw a dice if you want. Oh, oh! And I was also recentry tought how to do fancy formating, thanks dev {1}, you are da best! Also, I was told you people are using something called 'time' so I know how to use stopwatch now. AMAZING SOUND EFFECTS included! And I am here for {0}. Use /help for more.", UptimeExecutor.GetUptime(), SymbolFactory.AsString(SymbolFactory.Emoticon.Pleased))
                .BuildAndSend();
            return ExecutionResult.Success;
        }

    }

}
