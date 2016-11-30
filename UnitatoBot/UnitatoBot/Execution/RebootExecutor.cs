using System;
using System.Diagnostics;
using System.IO;
using BotCore.Bridge;
using BotCore.Execution;
using BotCore.Permission;
using Newtonsoft.Json.Linq;

namespace UnitatoBot.Execution {

    internal class RebootExecutor : IConditionalExecutonHandler, IInitializable {

        // IInitializable

        public void Initialize(ExecutionManager manager) {
            FileInfo fReboot = new FileInfo("reboot.json");

            if(fReboot.Exists)  {
                JObject jObject;
                using(StreamReader reader = fReboot.OpenText()) {
                    jObject = JObject.Parse(reader.ReadToEnd());
                }

                ServiceMessage msg = jObject.GetValue("Message").ToObject<ServiceMessage>();
                msg = manager.FindServiceType(msg.ServiceType)[0].FindMessage(msg.Origin, msg.Id);

                msg?.Edit(new ResponseBuilder()
                    .Text("Reboot requested by user")
                    .Block(jObject.GetValue("Issuer").ToObject<string>())
                    .Text("was completed!")
                    .Build());

                fReboot.Delete();
            }
        }

        // IExecutionHandler

        public string GetDescription() {
            return "(Restricted only to Admin permission group) Restart the bot";
        }

        public bool CanExecute(ExecutionContext context) {
            return Permissions.Can(context, Permissions.Reboot);
        }

        public bool Execute(ExecutionContext context) {
            ServiceMessage msg = context.ResponseBuilder
                .Username()
                .Text("requested reboot. Restart in progress...")
                .Send();

            SaveRebootFile(msg, context.Message.AuthorName);

            Process.Start(Process.GetCurrentProcess().MainModule.FileName.Replace(".vshost", string.Empty));
            Environment.Exit(0);
            return true;
        }

        // Util

        private void SaveRebootFile(ServiceMessage message, string issuer) {
            JObject jObject = new JObject {
                new JProperty("Issuer", issuer),
                new JProperty("Message", JToken.FromObject(message))
            };

            using(StreamWriter writer = new StreamWriter("reboot.json", false)) {
                writer.Write(jObject.ToString());
            }
        }

    }

}
