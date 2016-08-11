using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Text;

namespace UnitatoBot.Command.Execution.Executors {

    internal class FaggotStatsExecutor : IExecutionHandler, IInitializable {

        private JsonSerializer Serializer;
        private JObject JsonStatStorage;

        // IInitializable

        public void Initialize(CommandManager manager) {
            // Set up JsonSerializer
            this.Serializer = new JsonSerializer();
            this.Serializer.Formatting = Formatting.Indented;

            // Read storage file
            LoadStorageData();
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
                // Build common part of response
                ResponseBuilder builder = context.ResponseBuilder
                    .Block()
                        .Username()
                        .With("requested list of faggots")
                    .Block()
                    .Space();

                // If there are no entries in stats, builds response
                if(!JsonStatStorage.HasValues) {
                    builder.With(SymbolFactory.Emoticon.But).With("There are no faggots.").BuildAndSend();
                    return ExecutionResult.Success;
                }

                // Add each entry in stats to the response
                builder.MultilineBlock();
                foreach(JProperty property in JsonStatStorage.Properties()) {
                    int value = property.Value.ToObject<int>();
                    builder.With("{0} has {1} point{2}", property.Name, value, value > 1 ? "s" : string.Empty)
                           .NewLine();
                }
                builder.MultilineBlock();

                // Build response
                builder.BuildAndSend();
                return ExecutionResult.Success;
            }
            // Print out single statistic
            // /faggot stats [name]
            else if(context.Args[0].Equals("stats") && context.Args.Length == 2) {
                JProperty property = JsonStatStorage.Property(context.Args[1].ToLower());

                // Check if there is faggot in stats with name specified by command's second argument
                if(property == null) return ExecutionResult.Fail;

                // Gets the property value as int
                int value = property.Value.ToObject<int>();

                // Build response
                context.ResponseBuilder
                    .Block()
                        .Username()
                        .With("requested faggot statistics,")
                        .With("{0} has {1} point{2}", context.Args[1], value, value > 1 ? "s" : string.Empty)
                    .Block()
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

                // Add one faggotpoint
                property.Value = new JValue(property.Value.ToObject<int>() + 1);

                // Save stats to file
                SaveStorageData();

                // Build response
                context.ResponseBuilder
                    .Block()
                        .Username()
                        .With("marked {0} as faggot", context.Args[0])
                    .Block()
                    .BuildAndSend();

                return ExecutionResult.Success;
            }
        }

        // Helpers

        private void LoadStorageData() {
            if(File.Exists("faggotpoints.json")) {
                StreamReader storageFileReader = new StreamReader("faggotpoints.json", Encoding.UTF8);
                this.JsonStatStorage = JObject.Parse(storageFileReader.ReadToEnd());
                storageFileReader.Close();
            } else {
                this.JsonStatStorage = JObject.Parse("{}");
            }
        }

        private void SaveStorageData() {
            StreamWriter fileWriter = File.CreateText("faggotpoints.json");
            Serializer.Serialize(fileWriter, JsonStatStorage);
            fileWriter.Close();
        }

    }

}
