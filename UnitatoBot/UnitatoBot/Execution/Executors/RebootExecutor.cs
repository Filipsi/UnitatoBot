using UnitatoBot.Command;
using UnitatoBot.Permission;
using System.Diagnostics;
using System;
using System.IO;
using UnitatoBot.Bridge;
using Newtonsoft.Json;

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

                ServiceMessage msg = JsonConvert.DeserializeObject<ServiceMessage>(data);
                msg = manager.FindServiceType(msg.ServiceType)[0].FindMessage(msg.Origin, msg.Id);

                if(msg != null)
                    msg.Edit("Reboot complete!");

                fReboot.Delete();
            }
        }

        // IExecutionHandler

        public string GetDescription() {
            return "(Restricted only to Admin permission group) Restart the bot";
        }

        public ExecutionResult CanExecute(CommandContext context) {
            return Permissions.Has(context, Permissions.Reboot) ? ExecutionResult.Success : ExecutionResult.Denied;
        }

        public ExecutionResult Execute(CommandContext context) {
            ServiceMessage msg = context.ResponseBuilder
                .Username()
                .Text("requested reboot. Reboot in progress...")
                .Send();

            SaveRebootFile(msg);

            Process.Start(Process.GetCurrentProcess().MainModule.FileName.Replace(".vshost", string.Empty));
            Environment.Exit(0);
            return ExecutionResult.Success;
        }

        // Util

        private void SaveRebootFile(ServiceMessage message) {
            using(StreamWriter writer = new StreamWriter("reboot.json", false)) {
                writer.Write(JsonConvert.SerializeObject(message));
            }
        }

    }

}
