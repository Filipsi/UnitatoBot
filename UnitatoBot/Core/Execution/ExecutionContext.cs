using System.Text.RegularExpressions;
using BotCore.Bridge;
using BotCore.Util;

namespace BotCore.Execution {

    public class ExecutionContext {

        public ExecutionManager   ExecutionManager  { private set; get; }
        public ExecutionDispacher          ExecutionDispacher         { private set; get; }
        public string           CommandName     { private set; get; }
        public string[]         Args            { private set; get; }
        public bool             HasArguments    { get; }
        public string           RawArguments    { get; }
        public ServiceMessage   Message         { get; }
        public ResponseBuilder  ResponseBuilder => _response ?? (_response = new ResponseBuilder(Message));
        private ResponseBuilder _response;

        public ExecutionContext(ExecutionDispacher executionDispacher, ExecutionManager manager, ServiceMessage message) {
            ExecutionDispacher = executionDispacher;
            ExecutionManager = manager;
            Message = message;
            CommandName = Expressions.CommandParser.Capture(Message.Text, "ExecutionDispacher");
            HasArguments = Expressions.CommandArgumentParser.Test(Message.Text);

            RawArguments = Expressions.CommandArgumentParser.Capture(Message.Text, "args");
            if(!string.IsNullOrEmpty(RawArguments))
                RawArguments = Regex.Replace(RawArguments, @"\r\n?|\n", string.Empty);

            Args = HasArguments ? RawArguments.Split(' ') : null;
        }

    }

}
