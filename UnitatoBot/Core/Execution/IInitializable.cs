using BotCore.Command;

namespace BotCore.Execution {

    public interface IInitializable {

        void Initialize(CommandManager manager);

    }

}
