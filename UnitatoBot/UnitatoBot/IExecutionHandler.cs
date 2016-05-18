using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitatoBot {

    internal interface IExecutionHandler {

        void Initialize();

        string GetDescription();

        ExecutionResult CanExecute(CommandManager manager, CommandContext context);

        ExecutionResult Execute(CommandManager manager, CommandContext context);

    }

}
