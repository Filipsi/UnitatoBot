using System;
using System.Text;
using UnitatoBot.Connector;

namespace UnitatoBot.Command {

    internal class ResponseBuilder {

        private ConnectionMessage Message;
        private StringBuilder Builder;
        private bool HasNewline = false;

        public bool ShouldDeleteMessage { private set; get; }

        public ResponseBuilder(ConnectionMessage message, bool removeOriginalMessage = true) {
            this.Builder = new StringBuilder();
            this.ShouldDeleteMessage = removeOriginalMessage;
            this.Message = message;
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
            if(ShouldDeleteMessage) Message.Delete();

            // Build the responce
            string responce = Build();

            // Send responce to the client
            Message.ConnectionProvider.SendMessage(Message.Origin, responce);

            // Return the build responce string for further processing
            return responce;
        }

        public ConnectionMessage Send() {
            if(ShouldDeleteMessage)
                Message.Delete();

            return Message.ConnectionProvider.SendMessage(Message.Origin, Build());
        }

        // Content

        public ResponseBuilder With(string format, params object[] args) {
            if(Builder.Length > 0 && !HasNewline)
                Builder.Append(" ");

            Builder.Append(string.Format(format, args));
            HasNewline = false;
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
            With("*");
            return this;
        }

        public ResponseBuilder Bold() {
            With("**");
            return this;
        }

        public ResponseBuilder Strikeout() {
            With("~~");
            return this;
        }

        public ResponseBuilder Underline() {
            With("__");
            return this;
        }

        public ResponseBuilder Block() {
            With("`");
            return this;
        }

        public ResponseBuilder MultilineBlock() {
            With("```");
            return this;
        }

        public ResponseBuilder NewLine() {
            With(Environment.NewLine);
            HasNewline = true;
            return this;
        }

        // Utils

        public ResponseBuilder Username() {
            With(Message.Sender);
            return this;
        }

    }

}
