using UnitatoBot.Command;
using UnitatoBot.Permission;
using System.Diagnostics;
using System;
using System.IO;
using UnitatoBot.Bridge;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace UnitatoBot.Execution.Executors {

    internal class RebootExecutor : IExecutionHandler, IInitializable {

        // IInitializable

        public void Initialize(CommandManager manager) {
            FileInfo fReboot = new FileInfo("reboot.json");

            if(fReboot.Exists) {
                string data = string.Empty;
                using(StreamReader reader = fReboot.OpenText()) {
                    data = reader.ReadToEnd();
                }

                JObject jobject = JObject.Parse(data);

                ServiceMessage msg = jobject.GetValue("Message").ToObject<ServiceMessage>();
                msg = manager.FindServiceType(msg.ServiceType)[0].FindMessage(msg.Origin, msg.Id);

                if(msg != null)
                    msg.Edit(new ResponseBuilder()
                        .Text("Reboot requested by user")
                        .Block(jobject.GetValue("Issuer").ToObject<string>())
                        .Text("was completed!")
                        .Build());

                fReboot.Delete();
            }
        }

        // IExecutionHandler

        public string GetDescription() {
            return "(Restricted only to Admin permission group) Restart the bot";
        }

        public ExecutionResult CanExecute(CommandContext context) {
            return Permissions.Can(context, Permissions.Reboot) ? ExecutionResult.Success : ExecutionResult.Denied;
        }

        public ExecutionResult Execute(CommandContext context) {
            ServiceMessage msg = context.ResponseBuilder
                .Username()
                .Text("requested reboot. Restart in progress...")
                .Send();

            SaveRebootFile(msg, context.ServiceMessage.Sender);

            Process.Start(Process.GetCurrentProcess().MainModule.FileName.Replace(".vshost", string.Empty));
            Environment.Exit(0);
            return ExecutionResult.Success;
        }

        // Util

        private void SaveRebootFile(ServiceMessage message, string issuer) {
            JObject jobject = new JObject();
            jobject.Add(new JProperty("Issuer", issuer));
            jobject.Add(new JProperty("Message", JToken.FromObject(message)));

            using(StreamWriter writer = new StreamWriter("reboot.json", false)) {
                writer.Write(jobject.ToString());
            }
        }

    }

}
