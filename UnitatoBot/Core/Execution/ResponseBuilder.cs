using System;
using System.Text;
using BotCore.Bridge;
using BotCore.Util.Symbol;

namespace BotCore.Execution {

    public class ResponseBuilder {

        private ServiceMessage      Message     = null;
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

        public ResponseBuilder(ServiceMessage message, bool removeOriginalMessage = true) {
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
            Message.Service.SendMessage(Message.Origin, responce);

            return responce;
        }

        public ServiceMessage Send() {
            if(Message == null)
                return null;

            if(ShouldDeleteMessage)
                Message.Delete();

            return Message.Service.SendMessage(Message.Origin, Build());
        }

        // Content

        public ResponseBuilder Text(string format, params object[] args) {
            if(Builder.Length > 0 && !SkipSpace)
                Builder.Append(" ");

            Builder.Append(string.Format(format, args));
            SkipSpace = false;

            return this;
        }

        public ResponseBuilder Text(object entry) {
            return Text("{0}", entry);
        }

        public ResponseBuilder Text(char character, short repeat = 1) {
            for(short i = 0; i < repeat; i++) { Builder.Append(character); }
            return this;
        }

        public ResponseBuilder Text(Emoji emoji) {
            Text(SymbolFactory.AsString(emoji));
            return this;
        }

        public ResponseBuilder Text(Emoticon emoticon) {
            Text(SymbolFactory.AsString(emoticon));
            return this;
        }

        public ResponseBuilder Space(short repeat = 1) {
            return Text(' ', repeat);
        }

        // Formating

        public ResponseBuilder Italic() {
            Text("*");
            return this;
        }

        public ResponseBuilder Bold() {
            Text("**");
            return this;
        }

        public ResponseBuilder Strikeout() {
            Text("~~");
            SkipSpace = true;
            return this;
        }

        public ResponseBuilder Underline() {
            Text("__");
            return this;
        }

        public ResponseBuilder Block() {
            Text("`");
            return this;
        }

        public ResponseBuilder MultilineBlock() {
            Text("```");
            SkipSpace = true;
            return this;
        }

        public ResponseBuilder NewLine() {
            Text(Environment.NewLine);
            SkipSpace = true;
            return this;
        }

        // Formating contnet

        public ResponseBuilder MultilineBlock(string format, params object[] args) {
            MultilineBlock().Text(format, args).MultilineBlock();
            return this;
        }

        public ResponseBuilder MultilineBlock(object entry) {
            MultilineBlock().Text(entry).MultilineBlock();
            return this;
        }

        public ResponseBuilder Block(string format, params object[] args) {
            Block().Text(format, args).Block();
            return this;
        }

        public ResponseBuilder Block(object entry) {
            Block().Text(entry).Block();
            return this;
        }

        public ResponseBuilder Bold(string format, params object[] args) {
            Bold().Text(format, args).Bold();
            return this;
        }

        public ResponseBuilder Bold(object entry) {
            Bold().Text(entry).Bold();
            return this;
        }

        public ResponseBuilder Italic(string format, params object[] args) {
            Italic();
            SkipSpace = true;
            Text(format, args);
            SkipSpace = true;
            Italic();
            return this;
        }

        public ResponseBuilder Italic(object entry) {
            Italic();
            SkipSpace = true;
            Text(entry);
            SkipSpace = true;
            Italic();
            return this;
        }

        // Utils

        public ResponseBuilder Username() {
            Block().Text(Message.Sender).Block();
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
                Builder.Append(columnContent.Length > tableCellWidth ? columnContent.Remove(tableCellWidth - 3) + ".." : columnContent);
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
