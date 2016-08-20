using System;
using System.Text;
using UnitatoBot.Connector;

namespace UnitatoBot.Command {

    internal class ResponseBuilder {

        private ConnectionMessage   Message;
        private StringBuilder       Builder     = new StringBuilder();
        private bool                HasNewline  = false;

        public bool ShouldDeleteMessage { private set; get; }

        public ResponseBuilder(ConnectionMessage message, bool removeOriginalMessage = true) {
            ShouldDeleteMessage = removeOriginalMessage;
            Message = message;
	    }

        public ResponseBuilder() {
            ShouldDeleteMessage = false;
        }

        // Build

        public string Build() {
            return Builder.ToString();
        }

        public string BuildAndSend() {
            if(Message == null)
                return Build();

            if(ShouldDeleteMessage)
                Message.Delete();

            string responce = Build();
            Message.ConnectionProvider.SendMessage(Message.Origin, responce);

            return responce;
        }

        public ConnectionMessage Send() {
            if(Message == null)
                return null;

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

        public ResponseBuilder Clear() {
            Builder.Clear();
            return this;
        }

        public ResponseBuilder KeepSourceMessage() {
            ShouldDeleteMessage = false;
            return this;
        }

    }

}
