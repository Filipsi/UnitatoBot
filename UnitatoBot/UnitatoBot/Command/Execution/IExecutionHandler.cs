using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitatoBot.Command;

namespace UnitatoBot.Command.Execution {

    internal interface IExecutionHandler {

        string GetDescription();

        ExecutionResult CanExecute(CommandContext context);

        ExecutionResult Execute(CommandContext context);

    }

}
