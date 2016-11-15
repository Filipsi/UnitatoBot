using BotCore.Bridge;
using BotCore.Command;
using BotCore.Execution;
using BotCore.Util.Symbol;
using System.Collections.Generic;

namespace Unitato.Execution {

    internal class HelpExecutor : IExecutionHandler {

        // IExecutionHandler

        public string GetDescription() {
            return "Shows help, obviously.Use command name or alias as argument to show help only for specified command.";
        }

        public bool CanExecute(CommandContext context) {
            return true;
        }

        public bool Execute(CommandContext context) {
            if(!context.HasArguments) {
                PrintHelpInfo(context);
                return true;

            } else if(context.Args.Length == 1) {
                Command command = context.CommandManager.FindCommand(context.Args[0].ToLower());

                if(command != null) {
                    PrintHelpFor(context, command);
                    return true;
                }
            }

            return false;
        }

        // Logic

        private void PrintHelpFor(CommandContext context, Command command) {
            ResponseBuilder builder = context.ResponseBuilder
                .Username()
                .Text("here is help for command")
                .Block(command.Name)
                .Text(Emoticon.Magic)
                .NewLine()
                .NewLine();

            BuildCommandInfo(builder, command);

            if(builder.Length > 0)
                builder.Send();
        }

        private void PrintHelpInfo(CommandContext context) {
            ResponseBuilder builder = context.ResponseBuilder
                .Username()
                .Text("here is a list of stuff I can do: ")
                .Text(Emoticon.Magic)
                .NewLine()
                .NewLine();

            foreach(Command entry in context.CommandManager) {
                BuildCommandInfo(builder, entry);
            }

            if(builder.Length > 0)
                builder.Send();
        }

        private void BuildCommandInfo(ResponseBuilder builder, Command command) {
            LinkedList<IExecutionHandler>.Enumerator enumerator = command.GetExecutorsEnumerator();
            while(enumerator.MoveNext()) {

                // Split responce into multiple messages if responce length is greater then maximal responce length
                if(builder.Length + enumerator.Current.GetDescription().Length + 50 >= 2000) {
                    builder.Send();
                    builder.Clear().KeepSourceMessage();
                }

                builder
                    .Block("!{0}", command.Name)
                    .Text("{0}: {1}",
                        command.Aliases.Count > 0 ? "(alias: " + string.Join(", ", command.Aliases) + ")" : string.Empty,
                        enumerator.Current.GetDescription())
                    .NewLine();
            }
        }

    }

}
