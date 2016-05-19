using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitatoBot.Command;

namespace UnitatoBot.Execution {

    internal interface IExecutionHandler {

        void Initialize();

        string GetDescription();

        ExecutionResult CanExecute(CommandContext context);

        ExecutionResult Execute(CommandManager manager, CommandContext context);

    }

}
