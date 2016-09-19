using System.Text.RegularExpressions;
using UnitatoBot.Bridge;

namespace UnitatoBot.Command {

    internal class CommandContext {

        private ResponseBuilder _response;
        public ResponseBuilder ResponseBuilder {
            get {
                if(_response == null) _response = new ResponseBuilder(ServiceMessage);
                return _response;
            }
        }

        public ServiceMessage    ServiceMessage   { private set; get; }
        public CommandManager    CommandManager  { private set; get; }
        public Command           ExecutedCommand { private set; get; }
        public string            ExecutionName   { private set; get; }
        public bool              HasArguments    { private set; get; }
        public string[]          Args            { private set; get; }
        public string            RawArguments    { private set; get; }

        public CommandContext(Command command, CommandManager manager, ServiceMessage message) {
            ExecutedCommand = command;
            CommandManager = manager;
            ServiceMessage = message;
            ExecutionName = Expressions.CommandParser.Capture(ServiceMessage.Text, "command");
            HasArguments = Expressions.CommandArgumentParser.Test(ServiceMessage.Text);

            RawArguments = Expressions.CommandArgumentParser.Capture(ServiceMessage.Text, "args");
            if(RawArguments != null && RawArguments != string.Empty)
                RawArguments = Regex.Replace(RawArguments, @"\r\n?|\n", string.Empty);

            Args = HasArguments ? RawArguments.Split(' ') : null;
        }

    }

}
