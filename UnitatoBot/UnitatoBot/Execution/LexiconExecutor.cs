using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using BotCore.Execution;
using Newtonsoft.Json;
using RestSharp;

namespace UnitatoBot.Execution {

    internal class LexiconExecutor : IInitializable, IConditionalExecutonHandler {

        private RestClient  _lexiconClient;
        private RestRequest _requestGetMenu;
        private RestRequest _requestGetArticle;

        // IInitializable

        public void Initialize(ExecutionManager manager) {
            _lexiconClient = new RestClient("http://lexicon.filipsi.net/php");
            _requestGetMenu = new RestRequest("menu/processor.php", Method.GET);
            _requestGetArticle = new RestRequest("articles/processor.php", Method.GET);
        }

        // IExecutionHandler

        public string GetDescription() {
            return "Retrives entry form http://lexicon.filipsi.net/ using it's name as a argument or prints out articles from lexicon using 'list' as a argument";
        }

        public bool CanExecute(ExecutionContext context) {
            return context.HasArguments;
        }

        public bool Execute(ExecutionContext context) {
            if(context.Args[0] == "list") {
                _lexiconClient.ExecuteAsync(_requestGetMenu, response => {
                    // Parse data from server into list of ArticleEntries
                    List<MenuEntry> menuEntries = JsonConvert.DeserializeObject<List<MenuEntry>>(response.Content);

                    // Print out list of articles separated by commas
                    context.ResponseBuilder
                        .Username()
                        .Text("here is a list of articles that are available at http://lexicon.filipsi.net")
                        .MultilineBlock(string.Join(", ", menuEntries.Select(x => x.title)))
                        .BuildAndSend();
                });

                return true;
            } else {
                // Strips the /ExecutionDispacher from the arguments, this is here in order to enable reqests for articles with spaces in title
                string strippedArgument = context.Message.Text.Substring(1 + context.CommandName.Length + 1);

                _requestGetArticle.AddParameter("title", strippedArgument);
                _lexiconClient.ExecuteAsync(_requestGetArticle, response => {

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
                    // Remove newlines
                    articleText = articleText.Replace(Environment.NewLine, string.Empty);

                    context.ResponseBuilder
                        .Username()
                        .Text("requested article")
                        .Block(article.title)
                        .Text("from Lexicon")
                        .Text("http://lexicon.filipsi.net/#article/{0}", article.id)
                        .MultilineBlock(articleText)
                        .BuildAndSend();
                });

                return true;
            }
        }

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
