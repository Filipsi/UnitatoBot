using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace UnitatoBot.Command.Execution.Executors {

    internal class LexiconExecutor : IInitializable, IExecutionHandler{

        private RestClient LexiconClient;
        private RestRequest RequestGetMenu;
        private RestRequest RequestGetArticle;

        // IInitializable

        public void Initialize(CommandManager manager) {
            LexiconClient = new RestClient("http://lexicon.filipsi.net/php");
            RequestGetMenu = new RestRequest("menu/processor.php", Method.GET);
            RequestGetArticle = new RestRequest("articles/processor.php", Method.GET);
        }

        // IExecutionHandler

        public string GetDescription() {
            return "Retrives entry form http://lexicon.filipsi.net/ using it's name as a argument or prints out articles from lexicon using 'list' as a argument";
        }

        public ExecutionResult CanExecute(CommandContext context) {
            return context.HasArguments ? ExecutionResult.Success : ExecutionResult.Denied;
        }

        public ExecutionResult Execute(CommandContext context) {
            if(context.Args[0] == "list") {
                LexiconClient.ExecuteAsync(RequestGetMenu, response => {
                    // Parse data from server into list of ArticleEntries
                    List<MenuEntry> menuEntries = JsonConvert.DeserializeObject<List<MenuEntry>>(response.Content);

                    // Print out list of articles separated by commas
                    context.ResponseBuilder
                        .Block()
                            .Username()
                        .Block()
                        .With("here is a list of articles that are available at lexicon.filipsi.net:")
                        .MultilineBlock()
                            .With(String.Join(", ", menuEntries.Select(x => x.title)))
                        .MultilineBlock()
                        .BuildAndSend();
                });

                return ExecutionResult.Success;
            } else {
                // Strips the /command from the arguments, this is here in order to enable reqests for articles with spaces in title
                string strippedArgument = context.SourceMessage.Text.Substring(1 + context.ExecutionName.Length + 1);

                RequestGetArticle.AddParameter("title", strippedArgument);
                LexiconClient.ExecuteAsync(RequestGetArticle, response => {

                    // Check if there are data in the responce (should work, mostly ¯\_(ツ)_/¯)
                    if(response.Content.Equals(string.Empty) || response.Content.Equals("") || response.Content.Equals("[]")) return;

                    // Parse data from server into list of ArticleEntries, it has to be list even when
                    // responce contains only one element becouse server is allways returning it as an array
                    List<ArticleEntry> articleEntry = JsonConvert.DeserializeObject<List<ArticleEntry>>(response.Content);

                    // Get the one and only article entry
                    ArticleEntry article = articleEntry.First();

                    // Obtain text of the article
                    string articleText = article.text;
                    // Remove HTML tags
                    articleText = Regex.Replace(articleText, "<.*?>", String.Empty);
                    // Remove Empty lines
                    articleText = Regex.Replace(articleText, @"^\s+$[\r\n]*", "", RegexOptions.Multiline);

                    context.ResponseBuilder
                        .Block()
                            .Username()
                            .With("requested article {0} from Lexicon", article.title)
                        .Block()
                        .Space()
                        .With("http://lexicon.filipsi.net/#article/{0}", article.id)
                        .Space()
                        .MultilineBlock()
                            .With(articleText)
                        .MultilineBlock()
                        .BuildAndSend();
                });

                return ExecutionResult.Success;
            }
        }

        // Logic


        // JsonObjects
        // Used to parse data from server

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
