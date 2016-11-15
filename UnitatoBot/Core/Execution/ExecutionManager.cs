using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BotCore.Bridge;
using BotCore.Util;

namespace BotCore.Execution {

    public class ExecutionManager : IEnumerable<ExecutionDispacher> {

        public bool IsReady { private set; get; }

        private readonly IService[]      _services;
        private readonly List<ExecutionDispacher>   _commands;

        public ExecutionManager(params IService[] services) {
            _commands = new List<ExecutionDispacher>();
            IsReady = false;
            _services = services;

            foreach(IService service in _services) {
                service.OnMessageReceived += OnMessageReceived;
            }

            Logger.Log("ExecutionDispacher registration");
            Logger.SectionStart();
        }

        // IEnumerable

        public IEnumerator<ExecutionDispacher> GetEnumerator() {
            return _commands.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        // Logic

        private void OnMessageReceived(object sender, ServiceMessageEventArgs e) {
            if(!IsReady)
                return;

            if(!Expressions.CommandParser.Test(e.Message.Text))
                return;

            string commandName = Expressions.CommandParser.Capture(e.Message.Text, "ExecutionDispacher");
            ExecutionDispacher executionDispacher = _commands.Find(x => x.Name == commandName || x.IsMyAlias(commandName));

            if(executionDispacher != null)
                executionDispacher.Run(this, e.Message);
            else
                Logger.Log("ExecutionDispacher {0} not found", commandName);
        }

        public void Begin() {
            if(IsReady)
                return;

            Logger.SectionEnd();
            Logger.Log("Registred {0} ExecutionDispacher{1}", _commands.Count, _commands.Count > 1 ? "s" : string.Empty);
            Logger.Log("ExecutionDispacher inicialization");
            Logger.SectionStart();

            // Inicialize every executor
            foreach(ExecutionDispacher command in _commands) {
                foreach(IExecutionHandler executor in command)
                    if(executor is IInitializable) {
                        Logger.Log("{0} executor inicialization", executor.GetType().Name);
                        Logger.SectionStart();
                        ((IInitializable)executor).Initialize(this);
                        Logger.SectionEnd();
                    } else
                        Logger.Log("{0} executor is ready", executor.GetType().Name);
            }

            Logger.SectionEnd();
            Logger.Log("Commands initialized.");

            // Go into ready state
            IsReady = true;
        }

        public ExecutionManager RegisterCommand(string name, params IExecutionHandler[] handlers) {
            // ExecutionDispacher can be registerd only before initialization
            if(IsReady) {
                Logger.Error("Can't register {0} after ExecutionDispacher inicialization!", name);
                return this;
            }

            if(name.Equals(string.Empty) || !handlers.Any() || _commands.Exists(x => x.Name == name || x.IsMyAlias(name))) {
                Logger.Error("Failed to register ExecutionDispacher {0}!", name);
                return this;
            }

            // Create new ExecutionDispacher
            _commands.Add(new ExecutionDispacher(name, handlers));
            Logger.Log("Registered {0} with executor{1}:", name, handlers.Count() > 1 ? "s" : string.Empty);
            Logger.SectionStart();
            foreach(IExecutionHandler executor in handlers)
                Logger.Info(executor.GetType().ToString());
            Logger.SectionEnd();

            return this;
        }

        public ExecutionManager RegisterAlias(string name, string alias) {
            if(_commands.Exists(x => x.Name == alias || x.IsMyAlias(alias))) {
                Logger.Error("Can't register alias {0}, it is allready used!", alias);
                return this;
            }

            // Try to find ExecutionDispacher with given name or alias (you can add aliases using alias :P ... Wait, wat?)
            ExecutionDispacher executionDispacher = _commands.Find(x => x.Name == alias || x.IsMyAlias(alias));

            if(executionDispacher == null) {
                Logger.Error("Can't register alias {0}, no ExecutionDispacher named {1} was found!", alias, name);
                return this;
            }

            executionDispacher.AddAlias(alias);
            return this;
        }

        public ExecutionManager WithAlias(string alias) {
            if(_commands.Count == 0) {
                Logger.Error("Can't register alias {0}, no previous ExecutionDispacher registerd!", alias);
                return this;
            }

            if(_commands.Exists(x => x.Name == alias || x.IsMyAlias(alias))) {
                Logger.Error("Can't register alias {0}, it is allready used!", alias);
                return this;
            }

            _commands.Last().AddAlias(alias);
            return this;
        }

        public ExecutionDispacher FindCommand(string name) {
            return _commands.Find(x => x.Name == name || x.IsMyAlias(name));
        }

        public IService[] FindServiceType(string serviceType) {
            return Array.FindAll(_services, c => c.GetServiceType().Equals(serviceType));
        }

    }

}
