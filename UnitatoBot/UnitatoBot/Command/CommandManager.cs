using System;
using System.Collections.Generic;
using System.Linq;
using UnitatoBot.Bridge;
using UnitatoBot.Execution;

namespace UnitatoBot.Command {

    internal class CommandManager {

        public bool IsReady { private set; get; }

        private IService[]      Services;
        private List<Command>   Commands;

        public CommandManager(params IService[] services) {
            Commands = new List<Command>();
            IsReady = false;
            Services = services;

            foreach(IService service in Services) {
                service.OnMessageReceived += OnMessageReceived;
            }

            Logger.Log("Command registration");
            Logger.SectionStart();
        }

        private void OnMessageReceived(object sender, ServiceMessageEventArgs e) {
            if(!IsReady) return;

            bool isCommand = Expressions.CommandParser.Test(e.Message.Text);
            Logger.Log("Received {0} from {1}, IsCommand: {2}", e.Message.Text, e.Message.Sender, isCommand);
            if(!isCommand) return;

            string commandName = Expressions.CommandParser.Capture(e.Message.Text, "command");
            Command command = Commands.Find(x => x.Name == commandName || x.IsAlias(commandName));

            if(command != null) command.Execute(this, e.Message); else Logger.Log("Command {0} not found", commandName);
        }

        public void Begin() {
            if(IsReady) return;

            Logger.SectionEnd();
            Logger.Log("Registred {0} command{1}", Commands.Count, Commands.Count > 1 ? "s" : string.Empty);
            Logger.Log("Command inicialization");
            Logger.SectionStart();

            // Inicialize every executor
            foreach(var enumerator in Commands.Select(x => x.GetExecutorsEnumerator()).AsEnumerable()) {
                while(enumerator.MoveNext()) {
                    if(enumerator.Current is IInitializable) {
                        Logger.Log("{0} executor inicialization", enumerator.Current.GetType().Name);
                        Logger.SectionStart();
                            ((IInitializable)enumerator.Current).Initialize(this);
                        Logger.SectionEnd();
                        
                    } else {
                        Logger.Log("{0} executor is ready", enumerator.Current.GetType().Name);
                    }
                }
            }

            Logger.SectionEnd();
            Logger.Log("Commands initialized.");

            // Go into ready state
            IsReady = true;
        }

        public CommandManager RegisterCommand(string name, params IExecutionHandler[] handlers) {
            // Command can be registerd only before initialization
            if(IsReady) {
                Logger.Error("Can't register {0} after command inicialization!", name);
                return this;
            }

            if(name.Equals(string.Empty) || handlers.Count() == 0 || Commands.Exists(x => x.Name == name || x.IsAlias(name))) {
                Logger.Error("Failed to register command {0}!", name);
                return this;
            }

            // Create new command
            Commands.Add(new Command(name, handlers));
            Logger.Log("Registered {0} with executor{1}:", name, handlers.Count() > 1 ? "s" : string.Empty);
            Logger.SectionStart();
            foreach(IExecutionHandler executor in handlers) {
                Logger.Info(executor.GetType().ToString());
            }
            Logger.SectionEnd();

            return this;
        }

        public CommandManager RegisterAlias(string name, string alias) {
            if(Commands.Exists(x => x.Name == alias || x.IsAlias(alias))) {
                Logger.Error("Can't register alias {0}, it is allready used!", alias);
                return this;
            }

            // Try to find command with given name or alias (you can add aliases using alias :P ... Wait, wat?)
            Command command = Commands.Find(x => x.Name == alias || x.IsAlias(alias));

            if(command == null) {
                Logger.Error("Can't register alias {0}, no command named {1} was found!", alias, name);
                return this;
            }

            command.AddAlias(alias);
            return this;
        }

        public CommandManager WithAlias(string alias) {
            if(Commands.Count == 0) {
                Logger.Error("Can't register alias {0}, no previous command registerd!", alias);
                return this;
            }

            if(Commands.Exists(x => x.Name == alias || x.IsAlias(alias))) {
                Logger.Error("Can't register alias {0}, it is allready used!", alias);
                return this;
            }

            Commands.Last().AddAlias(alias);
            return this;
        }

        public List<Command>.Enumerator GetEnumerator() {
            return Commands.GetEnumerator();
        }

        public Command FindCommand(string name) {
            return Commands.Find(x => x.Name == name || x.IsAlias(name));
        }

        public IService[] FindServiceType(string serviceType) {
            return Array.FindAll(Services, c => c.GetServiceType().Equals(serviceType));
        }

    }

}
