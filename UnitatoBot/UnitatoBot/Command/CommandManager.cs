﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitatoBot.Connector;
using UnitatoBot.Execution;

namespace UnitatoBot.Command {

    internal class CommandManager {

        public IConnector ServiceConnector { private set; get; }
        public bool isInitialized { private set; get; }

        private Dictionary<string, IExecutionHandler> CommandExecutionMapping;
        private IExecutionHandler lastExecutor;

        public CommandManager(IConnector connector) {
            this.ServiceConnector = connector;
            this.isInitialized = false;
            this.CommandExecutionMapping = new Dictionary<string, IExecutionHandler>();

            // Bind IConnector's message event to the executors
            ServiceConnector.OnMessageReceived += OnMessageReceived;
        }

        private void OnMessageReceived(object sender, Discord.MessageEventArgs e) {
            if(!isInitialized) return;

            // Check if message is valid command or emoji
            bool isCommand = Expressions.CommandParser.Test(e.Message.Text);

            Console.WriteLine("Received {0} from {1}, IsCommand: {1}", e.Message.Text, e.User, isCommand);

            // Escape further processing if message is not command or emoji
            if(!isCommand) return;

            // Execute
            ExecuteCommand(new CommandContext(e.Message));
        }

        public void Initialize() {
            if(isInitialized) return;

            // TODO: Executors with aliases are inicialized multiple times, this shouldn't happen
            foreach(IExecutionHandler executor in CommandExecutionMapping.Values) {
                Console.WriteLine("Initializing executor: {0}", executor.GetType().Name);
                executor.Initialize();
            }

            Console.WriteLine("Commands initialized.");
            this.isInitialized = true;
        }

        public CommandManager RegisterCommand(string command, IExecutionHandler handler) {
            if(CommandExecutionMapping.ContainsKey(command) || command == string.Empty || handler == null) {
                Console.WriteLine("Failed to register command {0}!", command);
                return this;
            }

            CommandExecutionMapping.Add(command, handler);
            this.lastExecutor = handler;
            return this;
        }

        public CommandManager WithAlias(string command) {
            CommandExecutionMapping.Add(command, this.lastExecutor);
            return this;
        }

        public void ExecuteCommand(CommandContext context) {
            if(!isInitialized || !CommandExecutionMapping.ContainsKey(context.Command)) return;

            IExecutionHandler executor = CommandExecutionMapping[context.Command];
            ExecutionResult canExecute = executor.CanExecute(context);

            if(canExecute == ExecutionResult.Success) {
                ExecutionResult executionRes = executor.Execute(this, context);
                if(executionRes != ExecutionResult.Success) Console.WriteLine("Execution of {0} was not sucessful with result of {1}", context.Command, executionRes);
            } else {
                Console.WriteLine("Execution of {0} can not be started wit result of {1}", context.Command, canExecute);
            }
        }

        public Dictionary<string, IExecutionHandler>.Enumerator GetEnumerator() {
            return CommandExecutionMapping.GetEnumerator();
        }

    }

}