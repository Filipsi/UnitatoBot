using System;
using System.Text;
using UnitatoBot.Connector;

namespace UnitatoBot.Command {

    internal class ResponseBuilder {

        private ConnectionMessage   Message     = null;
        private StringBuilder       Builder     = new StringBuilder();
        private bool                SkipSpace   = false;

        private short               tableCellWidth  = 0;
        private string[]            tableColumns    = null;

        public bool ShouldDeleteMessage {
            private set; get;
        }

        public int Length {
            get { return Builder.Length; }
        }

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
            if(Builder.Length > 0 && !SkipSpace)
                Builder.Append(" ");

            Builder.Append(string.Format(format, args));
            SkipSpace = false;

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
            SkipSpace = true;
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
            SkipSpace = true;
            return this;
        }

        public ResponseBuilder NewLine() {
            With(Environment.NewLine);
            SkipSpace = true;
            return this;
        }

        // Formating contnet

        public ResponseBuilder MultilineBlock(string format, params object[] args) {
            MultilineBlock().With(format, args).MultilineBlock();
            return this;
        }

        public ResponseBuilder MultilineBlock(object entry) {
            MultilineBlock().With(entry).MultilineBlock();
            return this;
        }

        public ResponseBuilder Block(string format, params object[] args) {
            Block().With(format, args).Block();
            return this;
        }

        public ResponseBuilder Block(object entry) {
            Block().With(entry).Block();
            return this;
        }

        public ResponseBuilder Bold(string format, params object[] args) {
            Bold().With(format, args).Bold();
            return this;
        }

        public ResponseBuilder Bold(object entry) {
            Bold().With(entry).Bold();
            return this;
        }

        // Utils

        public ResponseBuilder Username() {
            Block().With(Message.Sender).Block();
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

        // Table

        public ResponseBuilder TableStart(short cellWidth, params string[] columnNames) {
            tableCellWidth = cellWidth;
            tableColumns = columnNames;

            MultilineBlock()
                .TableRow(tableColumns)
                .TableSpacer();

            return this;
        }

        public ResponseBuilder TableRow(params string[] column) {
            NewLine();

            for(short columnIndex = 0; columnIndex < tableColumns.Length; columnIndex++) {
                string columnContent = columnIndex < column.Length ? column[columnIndex] : string.Empty;

                Builder.Append(" ");
                Builder.Append(columnContent);
                for(int i = columnContent.Length + 1; i < tableCellWidth; i++) {
                    Builder.Append(" ");
                }

                if(columnIndex < tableColumns.Length - 1)
                    Builder.Append("|");
            }

            return this;
        }

        public ResponseBuilder TableSpacer() {
            NewLine();

            for(short i = 1; i < tableCellWidth * tableColumns.Length + tableColumns.Length; i++)
                Builder.Append(i % (tableCellWidth + 1) == 0 && i != 0 ? "|" : "-");

            return this;
        }

        public ResponseBuilder TableEnd() {
            tableCellWidth = 0;
            tableColumns = null;
            MultilineBlock();
            return this;
        }

    }

}
