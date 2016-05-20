using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitatoBot.Command {

    internal class ResponseBuilder {

        private bool RemoveOriginalMessage;
        private CommandContext Context;
        private StringBuilder Builder;

        public ResponseBuilder (CommandContext context, bool removeOriginalMessage = true) {
            this.Builder = new StringBuilder();
            this.RemoveOriginalMessage = removeOriginalMessage;
            this.Context = context;
	    }

        public string Build() {
            string response = Builder.ToString();
            if(RemoveOriginalMessage) Context.Message.Delete();
            Context.Message.Channel.SendMessage(response);
            return response;
        }

        public ResponseBuilder KeepOriginalMessage() {
            RemoveOriginalMessage = false;
            return this;
        }

        // Content

        public ResponseBuilder With(string format, params object[] args) {
            if(Builder.Length > 0) Builder.Append(" ");
            Builder.Append(string.Format(format, args));
            return this;
        }

        public ResponseBuilder With(object entry) {
            return With("{0}", entry);
        }

        public ResponseBuilder WithChar(char ch, short amount = 1) {
            for(short i = 0; i < amount; i++) { Builder.Append(ch); }
            return this;
        }

        public ResponseBuilder Space(short amount = 1) {
            return WithChar(' ', amount);
        }

        // Utils

        public ResponseBuilder Username() {
            With(Context.Message.User.Name);
            return this;
        }

        // Formating

        public ResponseBuilder Italic() {
            Builder.Append("*");
            return this;
        }

        public ResponseBuilder Bold() {
            Builder.Append("**");
            return this;
        }

        public ResponseBuilder Strikeout() {
            Builder.Append("~~");
            return this;
        }

        public ResponseBuilder Underline() {
            Builder.Append("__");
            return this;
        }

        public ResponseBuilder Block() {
            Builder.Append("`");
            return this;
        }

        public ResponseBuilder MultilineBlock() {
            Builder.Append("```");
            return this;
        }

        public ResponseBuilder NewLine() {
            Builder.Append(Environment.NewLine);
            return this;
        }

    }

}
