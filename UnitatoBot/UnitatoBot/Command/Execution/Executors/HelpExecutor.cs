﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitatoBot.Command;

namespace UnitatoBot.Command.Execution.Executors {

    internal class HelpExecutor : IExecutionHandler {

        // IExecutionHandler

        public void Initialize() {
            // NO-OP
        }

        public string GetDescription() {
            return "Shows help, obviously.";
        }

        public ExecutionResult CanExecute(CommandContext context) {
            return ExecutionResult.Success;
        }

        public ExecutionResult Execute(CommandContext context) {
            ResponseBuilder builder = context.ResponseBuilder
                .With("Sure,")
                .Username()
                .With("here is a list of stuff I can do: (ﾉ◕ヮ◕)ﾉ*:・ﾟ✧");

            builder.MultilineBlock();
            foreach(var entry in context.CommandManager) {
                builder.With("{0}: {1}", entry.Key, entry.Value.GetDescription())
                       .NewLine();
            }
            builder.MultilineBlock();

            builder.BuildAndSend();
            return ExecutionResult.Success;
        }
    }

}
