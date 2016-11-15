using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotCore.Execution {

    public interface IConditionalExecutonHandler : IExecutionHandler {

        bool CanExecute(ExecutionContext context);

    }
}
