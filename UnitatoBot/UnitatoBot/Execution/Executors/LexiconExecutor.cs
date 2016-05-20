using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using UnitatoBot.Command;
using UnitatoBot.Utilities.Http;

namespace UnitatoBot.Execution.Executors {

    class LexiconExecutor : IExecutionHandler {

        // IExecutionHandler

        public void Initialize() {
            //NO-OP
        }

        public string GetDescription() {
            return "Retrives entry form http://lexicon.filipsi.net/ using it's name as a argument or prints out articles from lexicon using 'list' as a argument";
        }

        public ExecutionResult CanExecute(CommandContext context) {
            return context.HasArguments ? ExecutionResult.Success : ExecutionResult.Denied;
        }

        public ExecutionResult Execute(CommandManager manager, CommandContext context) {
            if(context.Args[0] == "list") {
                // Make an request, there are no arguments required for this one, but QueryRequest requires at last pair
                Request request = Http.QueryRequest(HttpMethod.GET, "http://lexicon.filipsi.net/php/menu/processor.php", "", "");

                // Bind ReqestComplete event
                request.OnReqestComplete += (sender, args) => {
                    // Parse data from server into list of ArticleEntries
                    List<MenuEntry> menuEntries = JsonConvert.DeserializeObject<List<MenuEntry>>(args.Responce);

                    // Print out list of articles separated by commas
                    manager.ServiceConnector.Send(String.Join(", ", menuEntries.Select(x => x.title)));
                };

                return ExecutionResult.Success;
            } else {
                // Strips the /command from the arguments, this is here in order to enable reqests for articles with spaces in title
                string strippedArgument = context.RawCommand.Substring(1 + context.Command.Length + 1);

                // Make an reqest
                Request request = Http.QueryRequest(HttpMethod.GET, "http://lexicon.filipsi.net/php/articles/processor.php", "title", strippedArgument);

                // Bind ReqestComplete event
                request.OnReqestComplete += (sender, args) => {
                    // Check if there are data in the responce (should work, mostly ¯\_(ツ)_/¯)
                    if(args.Responce.Equals(string.Empty) || args.Responce.Equals("") || args.Responce.Equals("[]")) return;

                    // Parse data from server into list of ArticleEntries, it has to be list even when
                    // responce contains only one element becouse server is allways returning it as an array
                    List<ArticleEntry> articleEntry = JsonConvert.DeserializeObject<List<ArticleEntry>>(args.Responce);

                    // Get the one and only article entry
                    string article = articleEntry.First().text;
                    // Remove HTML tags
                    article = Regex.Replace(article, "<.*?>", String.Empty);
                    // Remove Empty lines
                    article = Regex.Replace(article, @"^\s+$[\r\n]*", "", RegexOptions.Multiline);

                    manager.ServiceConnector.Send(article);
                };

                return ExecutionResult.Success;
            }
        }

        // JsonObjects
        // Used to parse data from server using JsonConvert

        private class MenuEntry {
            public int id;
            public string title;
        }

        private class ArticleEntry {
            public int id;
            public string title;
            public string created;
            public string text;
        }

    }

}
