using System.Text.RegularExpressions;
using UnitatoBot.Connector;

namespace UnitatoBot.Command {

    internal class CommandContext {

        private ResponseBuilder _response;
        public ResponseBuilder ResponseBuilder {
            get {
                if(_response == null) _response = new ResponseBuilder(SourceMessage);
                return _response;
            }
        }

        public ConnectionMessage SourceMessage   { private set; get; }
        public CommandManager    CommandManager  { private set; get; }
        public Command           ExecutedCommand { private set; get; }
        public string            ExecutionName   { private set; get; }
        public bool              HasArguments    { private set; get; }
        public string[]          Args            { private set; get; }
        public string            RawArguments    { private set; get; }

        public CommandContext(Command command, CommandManager manager, ConnectionMessage message) {
            ExecutedCommand = command;
            CommandManager = manager;
            SourceMessage = message;
            ExecutionName = Expression.CommandParser.Capture(SourceMessage.Text, "command");
            HasArguments = Expression.CommandArgumentParser.Test(SourceMessage.Text);

            RawArguments = Expression.CommandArgumentParser.Capture(SourceMessage.Text, "args");
            if(RawArguments != null && RawArguments != string.Empty)
                RawArguments = Regex.Replace(RawArguments, @"\r\n?|\n", string.Empty);

            Args = HasArguments ? RawArguments.Split(' ') : null;
        }

    }

}
