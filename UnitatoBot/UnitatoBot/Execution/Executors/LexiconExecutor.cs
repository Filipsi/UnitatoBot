using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using UnitatoBot.Command;

namespace UnitatoBot.Execution.Executors {

    class LexiconExecutor : IExecutionHandler {

        // IExecutionHandler

        public void Initialize() {
            
        }

        public string GetDescription() {
            return "Retrives entry form http://lexicon.filipsi.net/ using it's name as a argument or prints out articles from lexicon using 'list' as a argument";
        }

        public ExecutionResult CanExecute(CommandContext context) {
            return context.HasArguments && context.Args.Length == 1 ? ExecutionResult.Success : ExecutionResult.Denied;
        }

        public ExecutionResult Execute(CommandManager manager, CommandContext context) {
            if(context.Args[0] == "list") {
                Utilities.HttpRequest.QueryRequest(Utilities.HttpRequest.HttpMethod.GET, "http://lexicon.filipsi.net/php/menu/processor.php", "", "").OnReqestComplete += (sender, args) => {
                    List<MenuEntry> menuEntries = JsonConvert.DeserializeObject<List<MenuEntry>>(args.Responce);
                    manager.ServiceConnector.Send(String.Join(", ", menuEntries.Select(x => x.title)));
                };
                return ExecutionResult.Success;
            } else {
                Utilities.HttpRequest.QueryRequest(Utilities.HttpRequest.HttpMethod.GET, "http://lexicon.filipsi.net/php/articles/processor.php", "title", context.Args[0]).OnReqestComplete += (sender, args) => {
                    if(args.Responce.Equals(string.Empty) || args.Responce.Equals("") || args.Responce.Equals("[]")) return;
                    List<ArticleEntry> articleEntry = JsonConvert.DeserializeObject<List<ArticleEntry>>(args.Responce);
                    manager.ServiceConnector.Send(Regex.Replace(articleEntry.First().text, "<.*?>", String.Empty));
                };
                return ExecutionResult.Success;
            }
        }

        // JsonObjects

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
