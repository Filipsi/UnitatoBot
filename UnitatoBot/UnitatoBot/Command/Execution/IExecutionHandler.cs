namespace UnitatoBot.Command.Execution {

    internal interface IExecutionHandler {

        string GetDescription();

        ExecutionResult CanExecute(CommandContext context);

        ExecutionResult Execute(CommandContext context);

    }

}
