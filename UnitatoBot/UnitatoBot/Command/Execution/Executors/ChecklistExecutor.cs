﻿using Newtonsoft.Json;
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

            Checklists = LoadAll(manager);
        }

        // IExecutionHandler

        public string GetDescription() {
            return "Create checklist with items that you can check out. To create a checklist use with argument 'create' [checklist-id] [title]. To add item to checklist use with argument 'add' [checklist-id](optional) [text]. To check or uncheck the item use with argument 'check' or 'uncheck' [checklist-id](optional) [item-index](from 0)";
        }

        public ExecutionResult CanExecute(CommandContext context) {
            return context.HasArguments ? ExecutionResult.Success : ExecutionResult.Fail;
        }

        public ExecutionResult Execute(CommandContext context) {
            switch(context.Args[0]) {
                case "create":
                    if(context.Args.Count() > 3 && !Checklists.Exists(c => c.Id.Equals(context.Args[1]))) {
                        Checklist checklist = new Checklist(context.Args[1], context.SourceMessage.Sender, context.RawArguments.Substring(context.RawArguments.IndexOf(context.Args[1]) + context.Args[1].Length + 1));

                        ConnectionMessage msg = context.ResponseBuilder
                            .Space()
                            .With(SymbolFactory.Emoji.Checklist)
                            .With("{0} (Checklist '{1}' by {2})", checklist.Title, checklist.Id, checklist.Owner)
                            .NewLine()
                                .With("        Use '/checklist add [checklist-id] [text]' to add items to the checklist by id or '/checklist add [text]' to add item to the last checklist created.")
                                .Send();

                        checklist.Message = msg;
                        Checklists.Add(checklist);

                        checklist.Save();

                        return ExecutionResult.Success;
                    }
                    break;

                case "add":
                    if(Checklists.Count > 0 && context.Args.Count() > 1) {
                        Checklist checklist = Checklists.Find(c => c.Id.Equals(context.Args[1]));

                        if(checklist == null) {
                            Checklists.Last().AddEntry(context.RawArguments.Substring(context.RawArguments.IndexOf(context.Args[0]) + context.Args[0].Length + 1));
                            context.SourceMessage.Delete();
                            return ExecutionResult.Success;
                        }

                        checklist.AddEntry(context.RawArguments.Substring(context.RawArguments.IndexOf(context.Args[1]) + context.Args[1].Length + 1));
                        context.SourceMessage.Delete();
                        return ExecutionResult.Success;
                    }
                    break;

                case "check":
                case "uncheck":
                    if(Checklists.Count > 0 && context.Args.Count() > 1) {
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
                checklist.UpdateMessage();

                if(result)
                    msg.Delete();

                return result;
            }

            return false;
        }

        private List<Checklist> LoadAll(CommandManager manager) {
            List <Checklist> list = new List<Checklist>();

            StreamReader reader;
            foreach(FileInfo file in new DirectoryInfo("checklist").GetFiles("*.json", SearchOption.TopDirectoryOnly)) {
                reader = new StreamReader(file.FullName);

                Checklist checklist = JsonConvert.DeserializeObject<Checklist>(reader.ReadToEnd());

                reader.Close();
                reader.Dispose();

                // This is only dummy message created by deserialization (contains Connection, Origin and Id)
                ConnectionMessage container = checklist.Message;

                IConnector connector = manager.FindConnector(container.Connection);
                if(connector != null) {
                    checklist.Message = connector.FindMessage(container.Origin, container.Id);
                    list.Add(checklist);
                }
            }

            Logger.Info("Loaded {0} entr{1}", list.Count, list.Count == 1 ? "y" : "ies");

            return list;
        }
 
    }
}
