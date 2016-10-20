using BotCore.Command;

namespace BotCore.Execution {

    public interface IExecutionHandler {

        string GetDescription();

        ExecutionResult CanExecute(CommandContext context);

        ExecutionResult Execute(CommandContext context);

    }

}
