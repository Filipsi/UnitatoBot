﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitatoBot.Command.Execution;
using UnitatoBot.Connector;

namespace UnitatoBot.Command {

    internal class CommandManager {

        public IConnector ServiceConnector { private set; get; }
        public bool       IsInitialized    { private set; get; }

        private List<Command> Commands;

        public CommandManager(IConnector connector) {
            this.ServiceConnector = connector;
            this.IsInitialized = false;
            this.Commands = new List<Command>();

            // Bind IConnector's message event
            ServiceConnector.OnMessageReceived += OnMessageReceived;
        }

        private void OnMessageReceived(object sender, ConnectionMessageEventArgs e) {
            if(!IsInitialized) return;

            // Check if message is valid command or emoji
            bool isCommand = Expressions.CommandParser.Test(e.Message.Text);

            Console.WriteLine("Received {0} from {1}, IsCommand: {2}", e.Message.Text, e.Message.Sender, isCommand);

            // Escape further processing if message is not command or emoji
            if(!isCommand) return;

            // Parse the command name
            string commandName = Expressions.CommandParser.Capture(e.Message.Text, "command");

            // Find Command that maches the name or alias
            Command command = Commands.Find(x => x.Name == commandName || x.IsAlias(commandName));

            // Execute, if there is such command
            if(command != null) command.Execute(this, e.Message);
        }

        public void Initialize() {
            // Inicialization can be only done once
            if(IsInitialized) return;

            // Inicialize every executor
            foreach(var enumerator in Commands.Select(x => x.GetExecutorsEnumerator()).AsEnumerable()) {
                while(enumerator.MoveNext()) {
                    if(enumerator.Current is IInitializable) {
                        ((IInitializable)enumerator.Current).Initialize();
                        Console.WriteLine("Executor {0} was initialized", enumerator.Current.GetType().Name);
                    } else {
                        Console.WriteLine("Executor {0} is ready", enumerator.Current.GetType().Name);
                    }
                }
            }

            Console.WriteLine("Commands initialized.");

            // Go into Initialized state
            this.IsInitialized = true;
        }

        public CommandManager RegisterCommand(string name, params IExecutionHandler[] handlers) {
            // Command can be registerd only before initialization
            if(IsInitialized) {
                Console.WriteLine("Can't register {0} after command inicialization!", name);
                return this;
            }

            // If input values are not valid or there allredy is such name for command, abort
            if(name.Equals(string.Empty) || handlers.Count() == 0 || Commands.Exists(x => x.Name == name || x.IsAlias(name))) {
                Console.WriteLine("Failed to register command {0}!", name);
                return this;
            }

            // Create new command
            Commands.Add(new Command(name, handlers));
            return this;
        }

        public CommandManager RegisterAlias(string name, string alias) {
            // If there allredy is such name for command, abort
            if(Commands.Exists(x => x.Name == alias || x.IsAlias(alias))) {
                Console.WriteLine("Can't register alias {0}, it is allready used!", alias);
                return this;
            }

            // Try to find command with given name or alias (you can add aliases using alias :P ... Wait, wat?)
            Command command = Commands.Find(x => x.Name == alias || x.IsAlias(alias));

            // If there is not a command with given name, abort
            if(command == null) {
                Console.WriteLine("Can't register alias {0}, no command named {1} was found!", alias, name);
                return this;
            }

            command.AddAlias(alias);
            return this;
        }

        public CommandManager WithAlias(string alias) {
            // If there are no commands registerd, abort
            if(Commands.Count == 0) return this;

            // If there allredy is such name for command, abort
            if(Commands.Exists(x => x.Name == alias || x.IsAlias(alias))) {
                Console.WriteLine("Can't register alias {0}, it is allready used!", alias);
                return this;
            }

            // Add new alias for lastest command
            Commands.Last().AddAlias(alias);
            return this;
        }

        public List<Command>.Enumerator GetEnumerator() {
            return Commands.GetEnumerator();
        }

        public Command GetCommand(string name) {
            return Commands.Find(x => x.Name == name || x.IsAlias(name));
        }

    }

}
