using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BotCore.Bridge;
using BotCore.Execution;
using BotCore.Permission;
using BotCore.Util;
using BotCore.Util.Symbol;
using Newtonsoft.Json.Linq;
using Unitato.Execution;
using UnitatoBot.Util.UsageManager;
using UnitatoBot.Util.FilipsiNetApi;

namespace UnitatoBot.Execution {

    internal class FaggotPointsExecutor : IExecutionHandler {

        // IExecutionHandler

        public string GetDescription() {
            return "Use with name as argument to mark someone as a faggot and add one faggot point. Use with argument 'list' to see statistics of all faggots. Use with argument 'stats' [name] to see statistics of given faggot. Use with argument 'purify' [name] [count](optional) to remove n faggot points from user (restricted). Use with argument 'clean' [name] to remove all faggot points (restricted)";
        }

        public bool CanExecute(ExecutionContext context) {
            return context.HasArguments;
        }

        public bool Execute(ExecutionContext context) {

            switch(context.Args[0]) {

                case "list":
                    if(context.Args.Length != 1)
                        return false;

                    FilipsiNetApi.GetFaggotPoints(jobject => {

                        if(!jobject.Properties().Any())
                            return;

                        context.ResponseBuilder
                            .Username()
                            .Text("wants to know how much of a faggots you people are.")
                            .TableStart(15, "Name", "Points");

                        foreach(JProperty prop in jobject.Properties().OrderByDescending(p => p.Value)) {

                            context.ResponseBuilder
                                .TableRow(prop.Name, prop.Value.ToString());

                        }

                        context.ResponseBuilder
                           .TableEnd()
                           .Send();
                    });

                    break;

                case "stats":
                    if(context.Args.Length != 2)
                        return false;

                    string name = context.Args[1].ToLower();

                    FilipsiNetApi.GetFaggotPoints(name, points => {

                        context.ResponseBuilder
                            .Username()
                            .Text("wants to know how much of a faggot")
                            .Block(name)
                            .Text("is. User has")
                            .Block(points)
                            .Text("point{0}", points > 1 ? "s." : ".")
                            .Send();

                    });

                    break;
            }

            return true;
        }

    }

}
