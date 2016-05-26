using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitatoBot.Command.Execution;
using UnitatoBot.Connector;

namespace UnitatoBot.Command {

    //TOTO: This is preparation for multiple executors support
    internal class Command {

        public string            Name     { private set; get; }
        public List<string>      Aliases  { private set; get; }
        public IExecutionHandler Executor { private set; get; }

        public Command(string name, IExecutionHandler executor) {
            this.Aliases = new List<string>();
            this.Name = name;
            this.Executor = executor;
        }

        public bool IsAlias(string name) {
            return Aliases.Contains(name);
        }

        public Command AddAlias(string alias) {
            if(!IsAlias(alias)) Aliases.Add(alias);
            return this;
        }

        public ExecutionResult Execute(CommandManager manager, ConnectionMessage message) {
            // Create a execution context for this command
            CommandContext context = new CommandContext(this, manager, message);

            // Check if command context is valid in order to be executied
            ExecutionResult result = Executor.CanExecute(context);

            // If command is valid, execute it, if not show error message
            if(result == ExecutionResult.Success) {
                result = Executor.Execute(context);
                Console.WriteLine("Execution of {0} ended with result {1}", context.CommandName, result);
            } else {
                Console.WriteLine("Execution of {0} failed the execution test with result {1}", context.CommandName, result);
            }

            return result;
        }

    }

}
