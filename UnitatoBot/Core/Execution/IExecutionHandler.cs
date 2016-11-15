namespace BotCore.Execution {

    public interface IExecutionHandler {

        string GetDescription();

        bool Execute(ExecutionContext context);

    }

}
