﻿using UnitatoBot.Connector;

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
            this.ExecutedCommand = command;
            this.CommandManager = manager;
            this.SourceMessage = message;
            this.ExecutionName = Expression.CommandParser.Capture(this.SourceMessage.Text, "command");
            this.HasArguments = Expression.CommandArgumentParser.Test(this.SourceMessage.Text);
            this.RawArguments = Expression.CommandArgumentParser.Capture(this.SourceMessage.Text, "args");
            this.Args = this.HasArguments ? RawArguments.Split(' ') : null;
        }

    }

}
