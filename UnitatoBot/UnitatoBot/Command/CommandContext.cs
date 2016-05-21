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

        public string   Command      { private set; get; }
        public bool     HasArguments { private set; get; }
        public string[] Args         { private set; get; }

        public CommandContext(CommandManager manager, ConnectionMessage message) {
            this.CommandManager = manager;
            this.Message = message;;
            this.Command = Expressions.CommandParser.Capture(this.Message.Text, "command");
            this.HasArguments = Expressions.CommandArgsParser.Test(this.Message.Text);
            this.Args = this.HasArguments ? Expressions.CommandArgsParser.Capture(this.Message.Text, "args").Split(' ') : null;
        }

        public void SendResponce(ResponseBuilder builder) {
            if(builder.DeleteMessage) Message.Delete();
            CommandManager.ServiceConnector.SendMessage(builder.Build());
        }

    }

}
