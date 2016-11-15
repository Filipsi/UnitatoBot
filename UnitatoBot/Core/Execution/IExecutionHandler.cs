using BotCore.Command;

namespace BotCore.Execution {

    public interface IExecutionHandler {

        string GetDescription();

        bool CanExecute(CommandContext context);

        bool Execute(CommandContext context);

    }

}
