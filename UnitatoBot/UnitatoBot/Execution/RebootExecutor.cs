using System.Diagnostics;
using System;
using System.IO;
using Newtonsoft.Json.Linq;
using BotCore.Execution;
using BotCore.Permission;
using BotCore.Bridge;
using BotCore.Command;

namespace Unitato.Execution {

    internal class RebootExecutor : IExecutionHandler, IInitializable {

        // IInitializable

        public void Initialize(CommandManager manager) {
            FileInfo fReboot = new FileInfo("reboot.json");

            if(fReboot.Exists) {
                string data = string.Empty;
                using(StreamReader reader = fReboot.OpenText()) {
                    data = reader.ReadToEnd();
                }

                JObject jObject = JObject.Parse(data);

                ServiceMessage msg = jObject.GetValue("Message").ToObject<ServiceMessage>();
                msg = manager.FindServiceType(msg.ServiceType)[0].FindMessage(msg.Origin, msg.Id);

                if(msg != null)
                    msg.Edit(new ResponseBuilder()
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

        public bool CanExecute(CommandContext context) {
            return Permissions.Can(context, Permissions.Reboot);
        }

        public bool Execute(CommandContext context) {
            ServiceMessage msg = context.ResponseBuilder
                .Username()
                .Text("requested reboot. Restart in progress...")
                .Send();

            SaveRebootFile(msg, context.Message.Sender);

            Process.Start(Process.GetCurrentProcess().MainModule.FileName.Replace(".vshost", string.Empty));
            Environment.Exit(0);
            return true;
        }

        // Util

        private void SaveRebootFile(ServiceMessage message, string issuer) {
            JObject jObject = new JObject();
            jObject.Add(new JProperty("Issuer", issuer));
            jObject.Add(new JProperty("Message", JToken.FromObject(message)));

            using(StreamWriter writer = new StreamWriter("reboot.json", false)) {
                writer.Write(jObject.ToString());
            }
        }

    }

}
