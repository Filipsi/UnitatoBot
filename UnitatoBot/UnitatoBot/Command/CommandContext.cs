using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitatoBot.Command {

    internal class CommandContext {

        public User User { private set; get; }
        public string Command { private set; get; }
        public bool HasArguments { private set; get; }
        public string[] Args { private set; get; }
        public string RawCommand { private set; get; }

        public CommandContext(Message msg) {
            this.User = msg.User;
            this.Command = Expressions.CommandParser.Capture(msg.Text, "command");
            this.HasArguments = Expressions.CommandArgsParser.Test(msg.Text);
            this.RawCommand = msg.Text;
            this.Args = this.HasArguments ? Expressions.CommandArgsParser.Capture(this.RawCommand, "args").Split(' ') : null;
        }

        public bool isEmoji() {
            return this.Command.Contains(":");
        }

    }

}
