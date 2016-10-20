using BotCore.Command;
using BotCore.Execution;
using BotCore.Util.Symbol;
using System;

namespace Unitato.Execution {

    internal class RussianRouletteExecutor : IExecutionHandler {

        private static readonly Random RNG = new Random();
        private bool[] Gun = new bool[6];
        private byte CylinderPointer = 0;

        // IExecutionHandler

        public string GetDescription() {
            return "Russian roulette, gun gas cylinder with 6 spaces for bullet. Use wihout argument to fire gun at yourself. Use with argument 'reload' to reaload the gun.";
        }

        public ExecutionResult CanExecute(CommandContext context) {
            return !context.HasArguments || (context.Args.Length == 1 && context.Args[0].Equals("reload")) ? ExecutionResult.Success : ExecutionResult.Denied;
        }

        public ExecutionResult Execute(CommandContext context) {

            if(context.HasArguments && context.Args[0].Equals("reload")) {
                Reload();

                context.ResponseBuilder
                    .Username()
                    .Text("reloaded gun with")
                    .Block("1")
                    .Text("bullet and spun the cilinder.")
                    .Send();

                return ExecutionResult.Success;
            } else {
                if(IsEmpty()) {
                    context.ResponseBuilder
                       .Text("Cilinder was empty, gun was reloaded with")
                       .Block("1")
                       .Text("bullet.")
                       .NewLine();

                    Reload();
                }

                if(Fire()) {
                    context.ResponseBuilder
                        .Text(Emoji.Bang)
                        .Space(2)
                        .Text(Emoji.Gun);
                } else {
                    context.ResponseBuilder
                        .Italic("click")
                        .Space()
                        .Text(Emoji.Gun);
                }

                context.ResponseBuilder
                    .Space(3)
                    .Username()
                    .Text("took the gun and pulled the trigger.")
                    .NewLine();

                context.ResponseBuilder.Send();
                return ExecutionResult.Success;
            }

        }

        // Logic

        private void Reload() {
            Array.Clear(Gun, 0, Gun.Length);
            Gun[RNG.Next(0, Gun.Length)] = true;
            CylinderPointer = 0;
        }

        private bool Fire() {
            bool result = Gun[CylinderPointer];

            if(result)
                Gun[CylinderPointer] = false;

            CylinderPointer++;

            return result;
        }

        private bool IsEmpty() {
            return Array.FindAll(Gun, element => element == true).Length == 0;
        }

    }

}
