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

namespace UnitatoBot.Execution {

    internal class FaggotPointsExecutor : IExecutionHandler, IInitializable {

        private JObject _statStorage;
        private UsageManager _usageManager;

        // IInitializable

        public void Initialize(ExecutionManager manager) {
            LoadFrom(new FileInfo("faggotpoints.json"));
        }

        // IExecutionHandler

        public string GetDescription() {
            return "Use with name as argument to mark someone as a faggot and add one faggot point. Use with argument 'list' to see statistics of all faggots. Use with argument 'stats' [name] to see statistics of given faggot. Use with argument 'purify' [name] [count](optional) to remove n faggot points from user (restricted). Use with argument 'clean' [name] to remove all faggot points (restricted)";
        }

        public bool CanExecute(ExecutionContext context) {
            return context.HasArguments;
        }

        public bool Execute(ExecutionContext context) {
            // Print out all statistics
            // faggot list
            if(context.Args[0].Equals("list") && context.Args.Length == 1) {
                ResponseBuilder builder = context.ResponseBuilder
                    .Username()
                    .Text("wants to know how much of a faggots you people are.");

                if(!_statStorage.HasValues) {
                    builder
                        .NewLine()
                        .Space(8)
                        .Text(Emoticon.But)
                        .Text("There are no faggots.")
                        .BuildAndSend();
                    return true;
                }

                builder.TableStart(20, "Name", "Points");

                var sorted = new List<JProperty>(_statStorage.Properties());
                sorted.Sort((x, y) => y.Value.ToObject<int>().CompareTo(x.Value.ToObject<int>()));
                foreach(JProperty property in sorted) {
                    builder.TableRow(property.Name, property.Value.ToString());
                }

                builder.TableEnd()
                    .Send();
                return true;
            }
            // Print out single statistic
            // faggot stats [name]
            else if(context.Args[0].Equals("stats") && context.Args.Length == 2) {
                string name = context.Args[1].ToLower();
                JProperty property = _statStorage.Property(name);

                if(property != null) {
                    int points = property.Value.ToObject<int>();

                    context.ResponseBuilder
                        .Username()
                        .Text("wants to know how much of a faggot")
                        .Block(name)
                        .Text("is. User has")
                        .Block(points)
                        .Text("point{0}", points > 1 ? "s." : ".")
                        .Send();
                    return true;
                }
            // Remove one faggotpoint from user
            // faggot purify [name]
            } else if(context.Args[0].Equals("purify") && context.Args.Length > 1 && Permissions.Can(context, Permissions.FaggotPurify)) {
                string name = context.Args[1].ToLower();
                JProperty property = _statStorage.Property(name);
                short modifier = 1;

                if(property != null && (context.Args.Length == 2 || (context.Args.Length == 3 && short.TryParse(context.Args[2], out modifier)))) {
                    context.ResponseBuilder
                        .Text(Emoji.Dizzy)
                        .Username()
                        .Text("used his divine powers to purify")
                        .Block(name);

                    int points = property.Value.ToObject<int>();

                    if(points - modifier > 0) {
                        property.Value = new JValue(points - modifier);
                        context.ResponseBuilder
                            .Text("from been as much of a faggot! This removed ")
                            .Block(modifier)
                            .Text(" faggot points.");

                    } else {
                        property.Remove();
                        context.ResponseBuilder.Text("and he is not faggot anymore!");
                    }

                    Save();

                    context.ResponseBuilder.Send();
                    return true;
                }
            // Remove all faggotpoints from user
            // faggot clean [name]
            } else if(context.Args[0].Equals("clean") && context.Args.Length > 1 && Permissions.Can(context, Permissions.FaggotClean)) {
                List<string> success = new List<string>();

                foreach(string name in context.Args.Skip(1)) {
                    JProperty property = _statStorage.Property(name.ToLower());
                    if(property != null) {
                        success.Add(name.ToLower());
                        property.Remove();
                    }
                }

                if(success.Count > 0) {
                    Save();

                    context.ResponseBuilder
                        .Text(Emoji.Star)
                        .Username()
                        .Text("usused his divine powers to clean all signs of being a faggot from");

                    foreach(string name in success)
                        context.ResponseBuilder
                            .Block(name)
                            .Space();

                    context.ResponseBuilder
                        .Send();

                    return true;
                }

                return false;

            }
            // Add faggotpoint to user
            // faggot [name]
            else {
                UsageManager.Usage usage = _usageManager.Get(context.Message.Sender);

                if(usage.CanBeUsed) {
                    string name = context.Args[0].ToLower();
                    JProperty property = _statStorage.Property(name);

                    if(property == null) {
                        _statStorage.Add(name, new JValue(0));
                        property = _statStorage.Property(name);
                    }

                    usage.UseOnce();
                    property.Value = new JValue(property.Value.ToObject<int>() + 1);
                    Save();

                    context.ResponseBuilder
                        .Username()
                        .Text("marked")
                        .Block(name)
                        .Text("as faggot. You have");

                    if(usage.Count > 0) {
                        context.ResponseBuilder
                            .Block(usage.Count)
                            .Text("out of")
                            .Block(usage.Max);
                    } else {
                        context.ResponseBuilder
                            .Text("no");
                    }

                    context.ResponseBuilder
                        .Text("faggot points left to give.")
                        .Send();

                } else {
                    context.ResponseBuilder
                        .Username()
                        .Text("you are out of faggots to give. Counter will reset after")
                        .Block(new DateTime(usage.TimeUntilReset.Ticks).ToString(TimerExecutor.DatePattenTime))
                        .Send();
                }

                return true;
            }

            return false;
        }

        // Helpers

        private void LoadFrom(FileInfo file) {
            if(file.Exists) {
                JObject jObject;
                using(StreamReader storageFileReader = file.OpenText()) {
                    jObject = JObject.Parse(storageFileReader.ReadToEnd());
                }

                _statStorage = (JObject) jObject.GetValue("Stats");
                _usageManager = jObject.GetValue("Usage").ToObject<UsageManager>();

            } else {
                _statStorage = JObject.Parse("{}");
                _usageManager = new UsageManager(3, TimeSpan.FromDays(1));
            }

            Logger.Info("Loaded {0} faggot entr{1}", _statStorage.Count, _statStorage.Count == 1 ? "y" : "ies");
        }

        private void Save() {
            JObject jObject = new JObject();
            jObject.Add(new JProperty("Stats", _statStorage));
            jObject.Add(new JProperty("Usage", JToken.FromObject(_usageManager)));

            using(StreamWriter writer = new StreamWriter("faggotpoints.json", false)) {
                writer.Write(jObject.ToString());
            }
        }

    }

}
