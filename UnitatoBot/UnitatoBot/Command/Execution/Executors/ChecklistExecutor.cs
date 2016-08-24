using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitatoBot.Component.Checklist;
using UnitatoBot.Connector;

namespace UnitatoBot.Command.Execution.Executors {

    internal class ChecklistExecutor : IInitializable, IExecutionHandler {

        private List<Checklist> Checklists;

        // IInitializable

        public void Initialize(CommandManager manager) {
            if(!Directory.Exists("checklist"))
                Directory.CreateDirectory("checklist");

            Checklists = LoadSaves(manager);
        }

        // IExecutionHandler

        public string GetDescription() {
            return "Create checklist with items that you can check out. To create a checklist use with argument 'create' [checklist-id] [title]. To add item to checklist use with argument 'add' [checklist-id](optional) [text]. To check or uncheck the item use with argument 'check' or 'uncheck' [checklist-id](optional) [item-index](from 0). To destroy checklist use with argument 'destroy' [checklist-id]. To remove item from checklist use use with argument 'remove' [checklist-id](optional) [item-index](from 0). To add multiple items to checklist use with argument 'import' [checklist-id](optional) [separator](string that splits entries) [text] (example: '/checklist import - -item1 -item2 -item3'). ";
        }

        public ExecutionResult CanExecute(CommandContext context) {
            return context.HasArguments ? ExecutionResult.Success : ExecutionResult.Fail;
        }

        public ExecutionResult Execute(CommandContext context) {
            switch(context.Args[0]) {
                case "create":
                    if(context.Args.Length > 2 && !Checklists.Exists(c => c.Id.Equals(context.Args[1]))) {
                        Checklist checklist = new Checklist(context.Args[1], context.SourceMessage.Sender, context.RawArguments.Substring(context.RawArguments.IndexOf(context.Args[1]) + context.Args[1].Length + 1));

                        ConnectionMessage msg = context.ResponseBuilder
                            .Space()
                            .With(SymbolFactory.Emoji.Checklist)
                            .With("{0} (Checklist '{1}' by {2})", checklist.Title, checklist.Id, checklist.Owner)
                            .NewLine()
                                .With("        Use '/checklist add [text]' to add item to last checklist (example: '/checklist add Hello world')")
                                .With("        Use '/checklist import [separator] [text]' to add multiple items last checklist (example: '/checklist import - -item1-item2-item3')")
                                .Send();

                        checklist.Message = msg;
                        Checklists.Add(checklist);

                        checklist.Save();

                        return ExecutionResult.Success;
                    }
                    break;

                case "destroy":
                    if(Checklists.Count > 0 && context.Args.Length == 2) {
                        Checklist checklist = Checklists.Find(c => c.Id.Equals(context.Args[1]));

                        if(checklist != null) {
                            context.SourceMessage.Delete();
                            Checklists.Remove(checklist);
                            checklist.Delete();
                            checklist.UpdateMessage("Checklist was deleted, no further edits can be made.");
                            Logger.Info("Checklist {0} was deleted", checklist.Id);
                            return ExecutionResult.Success;
                        }
                    }
                    break;

                case "add":
                    if(Checklists.Count > 0 && context.Args.Length > 1) {
                        Checklist checklist = Checklists.Find(c => c.Id.Equals(context.Args[1]));

                        if(checklist == null) {
                            Checklists.Last().Add(context.RawArguments.Substring(context.RawArguments.IndexOf(context.Args[0]) + context.Args[0].Length + 1));
                            context.SourceMessage.Delete();
                            return ExecutionResult.Success;
                        }

                        checklist.Add(context.RawArguments.Substring(context.RawArguments.IndexOf(context.Args[1]) + context.Args[1].Length + 1));
                        context.SourceMessage.Delete();
                        return ExecutionResult.Success;
                    }
                    break;

                case "remove":
                    if(Checklists.Count > 0 && context.Args.Length > 1) {
                        Checklist checklist = Checklists.Find(c => c.Id.Equals(context.Args[1]));

                        bool hasId = true;
                        if(checklist == null) {
                            checklist = Checklists.Last();
                            hasId = false;
                        }

                        byte index;
                        if(byte.TryParse(context.Args[hasId ? 2 : 1], out index)) {
                            context.SourceMessage.Delete();
                            checklist.Remove(index);
                            return ExecutionResult.Success;
                        }
                    }
                    break;

                case "import":
                    if(Checklists.Count > 0 && context.Args.Length > 2) {
                        Checklist checklist = Checklists.Find(c => c.Id.Equals(context.Args[1]));

                        bool hasId = true;
                        if(checklist == null) {
                            checklist = Checklists.Last();
                            hasId = false;
                        }

                        string[] separator = new string[] { context.Args[hasId ? 2 : 1] };
                        string import = context.RawArguments.Substring(context.RawArguments.IndexOf(separator[0]) + separator[0].Length + 1).Trim();

                        foreach(string entry in import.Split(separator, StringSplitOptions.RemoveEmptyEntries)) {
                            checklist.Add(entry, false);
                        }

                        context.SourceMessage.Delete();
                        checklist.UpdateMessage();
                        return ExecutionResult.Success;
                    }
                    break;

                case "check":
                case "uncheck":
                    if(Checklists.Count > 0 && context.Args.Length > 1) {
                        Checklist checklist = Checklists.Find(c => c.Id.Equals(context.Args[1]));
                        bool checkState = context.Args[0] == "check";

                        if(checklist == null)
                            return CheckEntry(context.SourceMessage, Checklists.Last(), context.Args[1], checkState) ? ExecutionResult.Success : ExecutionResult.Fail;

                        return CheckEntry(context.SourceMessage, checklist, context.Args[2], checkState) ? ExecutionResult.Success : ExecutionResult.Fail;
                    }
                    break;
            }

            return ExecutionResult.Fail;
        }

        // Utilities

        private bool CheckEntry(ConnectionMessage msg, Checklist checklist, string index, bool state) {
            byte i;

            if(byte.TryParse(index, out i)) {
                bool result = checklist.SetEntryState(i, state, msg.Sender);
                
                if(result) {
                    checklist.UpdateMessage();
                    msg.Delete();
                }

                if(checklist.IsCompleted) {
                    Checklists.Remove(checklist);
                    checklist.Delete();
                    checklist.UpdateMessage("Checklist was completed, no further edits can be made.");
                    Logger.Info("Checklist {0} was deleted", checklist.Id);
                }

                return result;
            }

            return false;
        }

        private List<Checklist> LoadSaves(CommandManager manager) {
            Logger.SectionStart();

            List<Checklist> list = new List<Checklist>();
            foreach(FileInfo file in new DirectoryInfo("checklist").GetFiles("*.json", SearchOption.TopDirectoryOnly)) {
                Logger.Info("Loading file {0} ...", file);

                Checklist checklist = Checklist.LoadFrom(manager, file);

                if(checklist != null)
                    list.Add(checklist);
            }

            Logger.SectionEnd();
            Logger.Info("Loaded {0} entr{1}", list.Count, list.Count == 1 ? "y" : "ies");

            return list;
        }
 
    }
}
