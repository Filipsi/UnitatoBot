using System.Collections.Generic;
using UnitatoBot.Bridge;
using UnitatoBot.Execution;

namespace UnitatoBot.Command {

    internal class Command {

        public string       Name      { private set; get; }
        public List<string> Aliases   { private set; get; }

        private LinkedList<IExecutionHandler> ExecutorList;

        public Command(string name, params IExecutionHandler[] executors) {
            this.Aliases = new List<string>();
            this.ExecutorList = new LinkedList<IExecutionHandler>(executors);
            this.Name = name;
        }

        // Alias

        public bool IsAlias(string name) {
            return Aliases.Contains(name);
        }

        public Command AddAlias(string alias) {
            if(!IsAlias(alias)) Aliases.Add(alias);
            return this;
        }

        // Execution

        public LinkedList<IExecutionHandler>.Enumerator GetExecutorsEnumerator() {
            return ExecutorList.GetEnumerator();
        }

        public void Execute(CommandManager manager, ServiceMessage message) {
            // Create a execution context for this command
            CommandContext context = new CommandContext(this, manager, message);

            if(ExecutorList.Count < 1) {
                Logger.Error("No executors found while trying to execute {0} command. ", Name);
            }

            // Try to execute the command using all of its executors
            foreach(IExecutionHandler executor in ExecutorList) {
                // Check if command context is valid in order to be executied
                ExecutionResult result = executor.CanExecute(context);

                // If command is valid, execute it, if not show error message
                if(result == ExecutionResult.Success) {
                    result = executor.Execute(context);
                    Logger.Log("Execution using {0} of {1} ended with result {2}", executor.GetType().Name, context.ExecutionName, result);
                } else {
                    Logger.Log("Execution using {0} of {1} failed the execution test with result {2}", executor.GetType().Name, context.ExecutionName, result);
                }
            }

        }

    }

}
