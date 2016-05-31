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

        public CommandManager    CommandManager { private set; get; }
        public ConnectionMessage Message        { private set; get; }

        public Command  Command      { private set; get; }
        public string   CommandName  { private set; get; }
        public bool     HasArguments { private set; get; }
        public string[] Args         { private set; get; }

        public CommandContext(Command command, CommandManager manager, ConnectionMessage message) {
            this.Command = command;
            this.CommandManager = manager;
            this.Message = message;
            this.CommandName = Expressions.CommandParser.Capture(this.Message.Text, "command");
            this.HasArguments = Expressions.CommandArgsParser.Test(this.Message.Text);
            this.Args = this.HasArguments ? Expressions.CommandArgsParser.Capture(this.Message.Text, "args").Split(' ') : null;
        }

        public void SendResponce(ResponseBuilder builder) {
            if(builder.ShouldDeleteMessage) Message.Delete();
            Message.ConnectionProvider.SendMessage(builder.Build());
        }

    }

}
