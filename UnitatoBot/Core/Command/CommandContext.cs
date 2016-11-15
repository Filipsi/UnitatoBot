using System.Text.RegularExpressions;
using BotCore.Bridge;
using BotCore.Util;
using BotCore.Execution;

namespace BotCore.Command {

    public class CommandContext {

        private ResponseBuilder _response;
        public ResponseBuilder ResponseBuilder {
            get {
                if(_response == null) _response = new ResponseBuilder(Message);
                return _response;
            }
        }

        public ServiceMessage    Message         { private set; get; }
        public CommandManager    CommandManager  { private set; get; }
        public Command           Command         { private set; get; }
        public string            CommandName     { private set; get; }
        public bool              HasArguments    { private set; get; }
        public string[]          Args            { private set; get; }
        public string            RawArguments    { private set; get; }

        public CommandContext(Command command, CommandManager manager, ServiceMessage message) {
            Command = command;
            CommandManager = manager;
            Message = message;
            CommandName = Expressions.CommandParser.Capture(Message.Text, "command");
            HasArguments = Expressions.CommandArgumentParser.Test(Message.Text);

            RawArguments = Expressions.CommandArgumentParser.Capture(Message.Text, "args");
            if(RawArguments != null && RawArguments != string.Empty)
                RawArguments = Regex.Replace(RawArguments, @"\r\n?|\n", string.Empty);

            Args = HasArguments ? RawArguments.Split(' ') : null;
        }

    }

}
