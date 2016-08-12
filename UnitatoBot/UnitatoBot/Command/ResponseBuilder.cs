using System;
using System.Text;
using UnitatoBot.Connector;

namespace UnitatoBot.Command {

    internal class ResponseBuilder {

        private CommandContext Context;
        private StringBuilder Builder;

        public bool ShouldDeleteMessage { private set; get; }

        public ResponseBuilder (CommandContext context, bool removeOriginalMessage = true) {
            this.Builder = new StringBuilder();
            this.ShouldDeleteMessage = removeOriginalMessage;
            this.Context = context;
	    }

        public ResponseBuilder KeepCommandMessage() {
            ShouldDeleteMessage = false;
            return this;
        }

        // Build

        public string Build() {
            return Builder.ToString();
        }

        public string BuildAndSend() {
            // Delete original message if needed
            if(ShouldDeleteMessage) Context.SourceMessage.Delete();

            // Build the responce
            string responce = Build();

            // Send responce to the client
            Context.SourceMessage.ConnectionProvider.SendMessage(Context.SourceMessage.Origin, responce);

            // Return the build responce string for further processing
            return responce;
        }

        public ConnectionMessage Send() {
            if(ShouldDeleteMessage)
                Context.SourceMessage.Delete();

            return Context.SourceMessage.ConnectionProvider.SendMessage(Context.SourceMessage.Origin, Build()).Result;
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

        public ResponseBuilder With(char character, short repeat = 1) {
            for(short i = 0; i < repeat; i++) { Builder.Append(character); }
            return this;
        }

        public ResponseBuilder With(SymbolFactory.Emoji emoji) {
            With(SymbolFactory.AsString(emoji));
            return this;
        }

        public ResponseBuilder With(SymbolFactory.Emoticon emoticon) {
            With(SymbolFactory.AsString(emoticon));
            return this;
        }

        public ResponseBuilder Space(short repeat = 1) {
            return With(' ', repeat);
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

        // Utils

        public ResponseBuilder Username() {
            With(Context.SourceMessage.Sender);
            return this;
        }

        public ResponseBuilder NewLine() {
            Builder.Append(Environment.NewLine);
            return this;
        }

    }

}
