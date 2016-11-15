using System;
using System.Collections;
using System.Collections.Generic;
using BotCore.Bridge;
using BotCore.Execution;
using BotCore.Util;

namespace BotCore.Command {

    public class Command : IEnumerable<IExecutionHandler> {

        public string       Name      { private set; get; }
        public List<string> Aliases   { private set; get; }

        private LinkedList<IExecutionHandler> Executors;

        public Command(string name, params IExecutionHandler[] executors) {
            Aliases = new List<string>();
            Executors = new LinkedList<IExecutionHandler>(executors);
            Name = name;
        }

        // Alias

        public bool IsValidAlias(string name) {
            return Aliases.Contains(name);
        }

        public Command AddAlias(string alias) {
            if(!IsValidAlias(alias)) Aliases.Add(alias);
            return this;
        }

        // IEnumerable

        public IEnumerator<IExecutionHandler> GetEnumerator() {
            return Executors.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        // Execution

        public void Run(CommandManager manager, ServiceMessage message) {
            if(Executors.Count < 1)
                Logger.Error("No executors found while trying to execute {0} command. ", Name);

            // Create a execution context for this command
            CommandContext context = new CommandContext(this, manager, message);

            foreach(IExecutionHandler executor in Executors) {
                if(executor.CanExecute(context))
                    Logger.Log("Execution using {0} of {1} ended with result {2}", executor.GetType().Name, context.CommandName, executor.Execute(context));
                else
                    Logger.Log("Execution using {0} of {1} failed the execution test with result false", executor.GetType().Name, context.CommandName);
            }
        }

    }

}
