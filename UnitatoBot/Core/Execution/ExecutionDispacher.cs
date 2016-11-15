using System.Collections;
using System.Collections.Generic;
using BotCore.Bridge;
using BotCore.Util;

namespace BotCore.Execution {

    public class ExecutionDispacher : IEnumerable<IExecutionHandler> {

        public string                       Name    { get; }
        public WrappedEnumerable<string>    Alias   { get; }

        private readonly List<string>                   _aliases;
        private readonly LinkedList<IExecutionHandler>  _executors;

        public ExecutionDispacher(string name, params IExecutionHandler[] executors) {
            Name = name;

            _aliases = new List<string>(); 
            _executors = new LinkedList<IExecutionHandler>(executors);
            Alias = new WrappedEnumerable<string>(_aliases);
        }

        // Alias

        public bool IsMyAlias(string name) {
            return _aliases.Contains(name);
        }

        public ExecutionDispacher AddAlias(string alias) {
            if(!IsMyAlias(alias))
                _aliases.Add(alias);

            return this;
        }

        // IEnumerable

        public IEnumerator<IExecutionHandler> GetEnumerator() {
            return _executors.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        // Execution

        public void Run(ExecutionManager manager, ServiceMessage message) {
            if(_executors.Count < 1)
                Logger.Error("No executors found while trying to run {0} ExecutionDispacher. ", Name);

            // Create a execution context for this ExecutionDispacher
            ExecutionContext context = new ExecutionContext(this, manager, message);

            foreach(IExecutionHandler executor in _executors) {
                if (executor is IConditionalExecutonHandler && !((IConditionalExecutonHandler) executor).CanExecute(context)) {
                    Logger.Log("Execution using {0} of '{1}' failed conditional execution test with result False", executor.GetType().Name, context.CommandName);
                    return;
                }

                Logger.Log("Execution using {0} of '{1}' ended with result {2}", executor.GetType().Name, context.CommandName, executor.Execute(context));
            }
        }

    }

}
