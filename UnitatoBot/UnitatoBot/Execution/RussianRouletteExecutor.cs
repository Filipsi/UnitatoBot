using System;
using BotCore.Execution;
using BotCore.Util.Symbol;

namespace UnitatoBot.Execution {

    internal class RussianRouletteExecutor : IExecutionHandler {

        private static readonly Random  Rng = new Random();

        private readonly bool[]         _gun = new bool[6];
        private byte                    _cylinderPointer;

        // IExecutionHandler

        public string GetDescription() {
            return "Russian roulette, gun gas cylinder with 6 spaces for bullet. Use wihout argument to fire gun at yourself. Use with argument 'reload' to reaload the gun.";
        }

        public bool CanExecute(ExecutionContext context) {
            return !context.HasArguments || (context.Args.Length == 1 && context.Args[0].Equals("reload"));
        }

        public bool Execute(ExecutionContext context) {

            if(context.HasArguments && context.Args[0].Equals("reload")) {
                Reload();

                context.ResponseBuilder
                    .Username()
                    .Text("reloaded gun with")
                    .Block("1")
                    .Text("bullet and spun the cilinder.")
                    .Send();

                return true;
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
                return true;
            }

        }

        // Logic

        private void Reload() {
            Array.Clear(_gun, 0, _gun.Length);
            _gun[Rng.Next(0, _gun.Length)] = true;
            _cylinderPointer = 0;
        }

        private bool Fire() {
            bool result = _gun[_cylinderPointer];

            if(result)
                _gun[_cylinderPointer] = false;

            _cylinderPointer++;

            return result;
        }

        private bool IsEmpty() {
            return Array.FindAll(_gun, element => element == true).Length == 0;
        }

    }

}
