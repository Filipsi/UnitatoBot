using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitatoBot.Connector;

namespace UnitatoBot.Command {

    internal class CommandContext {

        public ResponseBuilder ResponseBuilder {
            get { return new ResponseBuilder(this); }
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
            this.ExecutionName = Expressions.CommandParser.Capture(this.SourceMessage.Text, "command");
            this.HasArguments = Expressions.CommandArgsParser.Test(this.SourceMessage.Text);
            this.RawArguments = Expressions.CommandArgsParser.Capture(this.SourceMessage.Text, "args");
            this.Args = this.HasArguments ? RawArguments.Split(' ') : null;
        }

    }

}
