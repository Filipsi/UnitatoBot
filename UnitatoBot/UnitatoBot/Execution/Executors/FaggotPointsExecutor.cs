using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnitatoBot.Command;
using UnitatoBot.Permission;
using UnitatoBot.Symbol;
using UnitatoBot.Util;

namespace UnitatoBot.Execution.Executors {

    internal class FaggotPointsExecutor : IExecutionHandler, IInitializable {

        private static JsonSerializer SERIALIZER;

        static FaggotPointsExecutor() {
            SERIALIZER = new JsonSerializer();
            SERIALIZER.Formatting = Formatting.Indented;
        }

        private JObject JsonStatStorage;

        // IInitializable

        public void Initialize(CommandManager manager) {
            LoadFrom(new FileInfo("faggotpoints.json"));
        }

        // IExecutionHandler

        public string GetDescription() {
            return "Use with name as argument to mark someone as a faggot and add one faggot point. Use with argument 'list' to see statistics of all faggots. Use with argument 'stats' and name of the user as a second argument to see statistics of that faggot. Use with argument 'purify' and name of the user to remove one faggot point (restricted). Use with argument 'clean' and name of the user to remove all faggot points (restricted)";
        }

        public ExecutionResult CanExecute(CommandContext context) {
            return context.HasArguments ? ExecutionResult.Success : ExecutionResult.Denied;
        }

        public ExecutionResult Execute(CommandContext context) {
            // Print out all statistics
            // faggot list
            if(context.Args[0].Equals("list") && context.Args.Length == 1) {
                ResponseBuilder builder = context.ResponseBuilder
                    .Username()
                    .Text("wants to know how much of a faggots you people are.");

                if(!JsonStatStorage.HasValues) {
                    builder
                        .NewLine()
                        .Space(8)
                        .Text(Emoticon.But)
                        .Text("There are no faggots.")
                        .BuildAndSend();
                    return ExecutionResult.Success;
                }

                builder.TableStart(20, "Name", "Points");

                var sorted = new List<JProperty>(JsonStatStorage.Properties());
                sorted.Sort((x, y) => y.Value.ToObject<int>().CompareTo(x.Value.ToObject<int>()));
                foreach(JProperty property in sorted) {
                    builder.TableRow(property.Name, property.Value.ToString());
                }

                builder.TableEnd()
                    .Send();
                return ExecutionResult.Success;
            }
            // Print out single statistic
            // faggot stats [name]
            else if(context.Args[0].Equals("stats") && context.Args.Length == 2) {
                string name = context.Args[1].ToLower();
                JProperty property = JsonStatStorage.Property(name);

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
                    return ExecutionResult.Success;
                }
            // Remove one faggotpoint from user
            // faggot purify [name]
            } else if(context.Args[0].Equals("purify") && context.Args.Length == 2 && Permissions.Can(context, Permissions.FaggotPurify)) {
                string name = context.Args[1].ToLower();
                JProperty property = JsonStatStorage.Property(name);

                if(property != null) {
                    context.ResponseBuilder
                        .Text(Emoji.Dizzy)
                        .Username()
                        .Text("used his divine powers to purify")
                        .Block(name);

                    int points = property.Value.ToObject<int>();

                    if(points - 1 > 0) {
                        property.Value = new JValue(points - 1);
                        context.ResponseBuilder.Text("from been as much of a faggot!");
                    } else {
                        property.Remove();
                        context.ResponseBuilder.Text("and he is not faggot anymore!");
                    }

                    Save();

                    context.ResponseBuilder.Send();
                    return ExecutionResult.Success;
                }
                // Remove all faggotpoints from user
                // faggot clean [name]
            } else if(context.Args[0].Equals("clean") && context.Args.Length == 2 && Permissions.Can(context, Permissions.FaggotClean)) {
                string name = context.Args[1].ToLower();
                JProperty property = JsonStatStorage.Property(name);

                if(property != null) {
                    context.ResponseBuilder
                        .Text(Emoji.Star)
                        .Username()
                        .Text("usused his divine powers to clean all signs of being a faggot from")
                        .Block(name);

                    property.Remove();
                    Save();

                    context.ResponseBuilder.Send();
                    return ExecutionResult.Success;
                }
            }
            // Add faggotpoint to user
            // faggot [name]
            else {
                string name = context.Args[0].ToLower();
                JProperty property = JsonStatStorage.Property(name);

                if(property == null) {
                    JsonStatStorage.Add(name, new JValue(0));
                    property = JsonStatStorage.Property(name);
                }

                property.Value = new JValue(property.Value.ToObject<int>() + 1);
                Save();

                context.ResponseBuilder
                    .Username()
                    .Text("marked")
                    .Block(name)
                    .Text("as faggot")
                    .Send();

                return ExecutionResult.Success;
            }

            return ExecutionResult.Fail;
        }

        // Helpers

        private void LoadFrom(FileInfo file) {
            if(file.Exists) {
                using(StreamReader storageFileReader = file.OpenText()) {
                    JsonStatStorage = JObject.Parse(storageFileReader.ReadToEnd());
                }
            } else {
                JsonStatStorage = JObject.Parse("{}");
            }

            Logger.Info("Loaded {0} faggot entr{1}", JsonStatStorage.Count, JsonStatStorage.Count == 1 ? "y" : "ies");
        }

        private void Save() {
            using(StreamWriter fileWriter = File.CreateText("faggotpoints.json")) {
                SERIALIZER.Serialize(fileWriter, JsonStatStorage);
            }
        }

    }

}
