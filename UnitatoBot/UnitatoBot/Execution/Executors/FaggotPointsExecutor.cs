using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using UnitatoBot.Command;

namespace UnitatoBot.Execution.Executors {

    internal class FaggotStatsExecutor : IExecutionHandler, IInitializable {

        private static JsonSerializer SERIALIZER;

        static FaggotStatsExecutor() {
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
            return "Use with name as argument to mark someone as a Faggot and add one faggot point. Use wihout an argument to see statistics of all faggots. Use with argument 'stats' and name of the user as a second argument to see statistics of that faggot.";
        }

        public ExecutionResult CanExecute(CommandContext context) {
            if(!context.HasArguments) return ExecutionResult.Success;
            if(context.Args[0].Equals("stats") && context.Args.Length == 2) return ExecutionResult.Success;
            if(context.Args.Length == 1) return ExecutionResult.Success;
            return ExecutionResult.Denied;
        }

        public ExecutionResult Execute(CommandContext context) {
            // Print out all statistics
            // /faggot
            if(!context.HasArguments) {
                ResponseBuilder builder = context.ResponseBuilder
                    .Username()
                    .With("wants to know how much of a faggots you people are.");

                if(!JsonStatStorage.HasValues) {
                    builder
                        .NewLine()
                        .Space(8)
                        .With(SymbolFactory.Emoticon.But)
                        .With("There are no faggots.")
                        .BuildAndSend();
                    return ExecutionResult.Success;
                }

                builder.TableStart(20, "Name", "Points");
                foreach(JProperty property in JsonStatStorage.Properties()) {
                    builder.TableRow(property.Name, property.Value.ToString());
                }

                builder.TableEnd()
                    .BuildAndSend();
                return ExecutionResult.Success;
            }
            // Print out single statistic
            // /faggot stats [name]
            else if(context.Args[0].Equals("stats") && context.Args.Length == 2) {
                JProperty property = JsonStatStorage.Property(context.Args[1].ToLower());

                if(property == null) return ExecutionResult.Fail;

                int points = property.Value.ToObject<int>();
                context.ResponseBuilder
                    .Username()
                    .With("wants to know how much of a faggot")
                    .Block(context.Args[1])
                    .With("are. User has")
                    .Block(points)
                    .With("point{0}", points > 1 ? "s." : ".")
                    .BuildAndSend();
                return ExecutionResult.Success;
            } 
            // Add faggotpoint to user
            // /faggot [name]
            else {
                string name = context.Args[0].ToLower();
                JProperty property = JsonStatStorage.Property(name);

                // If user is not in the stats, creates new entry
                if(property == null) {
                    JsonStatStorage.Add(name, new JValue(0));
                    property = JsonStatStorage.Property(name);
                }

                property.Value = new JValue(property.Value.ToObject<int>() + 1);

                Save();

                context.ResponseBuilder
                    .Username()
                    .With("marked")
                    .Block(context.Args[0])
                    .With("as faggot")
                    .BuildAndSend();

                return ExecutionResult.Success;
            }
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
