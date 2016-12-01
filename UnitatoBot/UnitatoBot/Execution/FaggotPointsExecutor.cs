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
using UnitatoBot.Util.WebApi;
using Newtonsoft.Json;

namespace UnitatoBot.Execution {

    internal class FaggotPointsExecutor : IInitializable, IExecutionHandler {

        private UsageManager _usageManager;

        // IInitializable

        public void Initialize(ExecutionManager manager) {
            _usageManager = UsageManager.Load("faggotpoints-usage.json", 3, TimeSpan.FromDays(1));
        }

        // IExecutionHandler

        public string GetDescription() {
            return "Use with name as argument to mark someone as a faggot and add one faggot point. Use with argument 'list' to see statistics of all faggots. Use with argument 'stats' [name] to see statistics of given faggot.";
        }

        public bool CanExecute(ExecutionContext context) {
            return context.HasArguments;
        }

        public bool Execute(ExecutionContext context) {

            switch(context.Args[0]) {

                case "list":
                    if(context.Args.Length != 1)
                        return false;

                    WebApi.GetFaggotPoints(jobject => {

                        if(!jobject.Properties().Any())
                            return;

                        context.ResponseBuilder
                            .Username()
                            .Text("requested list of faggot points.")
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

                    WebApi.GetFaggotPoints(name, points => {

                        context.ResponseBuilder
                            .Username()
                            .Text("wants to know how big faggot")
                            .Block(name)
                            .Text("is. User has")
                            .Block(points)
                            .Text("point{0}", points > 1 ? "s." : ".")
                            .Send();

                    });

                    break;

                default:
                    if(context.Args.Length != 1)
                        return false;

                    string namef = context.Args[0].ToLower();
                    UsageManager.Usage usage = _usageManager.Get(context.Message.AuthorName);

                    if(usage.CanBeUsed) {

                        WebApi.SetFaggotPoints(namef, 1, success => {

                            if(!success)
                                return;

                            usage.UseOnce();

                            context.ResponseBuilder
                                .Username()
                                .Text("marked")
                                .Block(namef)
                                .Text("as faggot. You have");

                            if(usage.Uses > 0)
                                context.ResponseBuilder
                                    .Block(usage.Uses)
                                    .Text("out of")
                                    .Block(usage.Manager.MaximumUses);
                            else
                                context.ResponseBuilder
                                    .Text("no");

                            context.ResponseBuilder
                                .Text("faggot points left to give.")
                                .Send();

                        });

                    } else
                        context.ResponseBuilder
                            .Username()
                            .Text("you are out of faggots to give. Counter will reset after")
                            .Block(new DateTime(usage.TimeUntilReset.Ticks).ToString(TimerExecutor.DatePattenTime))
                            .Send();

                    break;
            }

            return true;
        }

    }

}
