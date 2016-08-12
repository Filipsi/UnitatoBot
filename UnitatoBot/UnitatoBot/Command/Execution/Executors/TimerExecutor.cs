using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using UnitatoBot.Connector;

namespace UnitatoBot.Command.Execution.Executors {

    internal class TimerExecutor : IExecutionHandler {

        // IExecutionHandler

        public string GetDescription() {
            return "Create a timer that will count down from given time to zero in seconds. You can use multiple arguments and characters like 's' for seconds, 'm' as minutes and 'h' for hours. (ex: '/timer 5m 42s')";
        }

        public ExecutionResult CanExecute(CommandContext context) {
            return context.HasArguments ? ExecutionResult.Success : ExecutionResult.Fail;
        }

        public ExecutionResult Execute(CommandContext context) {

            int sec = 0;
            foreach(string argument in context.Args) {
                int res = GetSecondsFromInput(argument);
                if(res == -1) return ExecutionResult.Fail;

                sec += res;
            }

            

            Countdown countdown = new Countdown(TimeSpan.FromSeconds(sec));

            ConnectionMessage responce = context.ResponseBuilder
                .With(SymbolFactory.Emoji.Stopwatch)
                .With("Timer was set to")
                .With(sec)
                .With("seconds. Timer is running!")
                .Send();

            countdown.OnStateChanged += (sender, args) => {
                TimeSpan remining = args.Remining;
                if(remining.TotalSeconds > 0) {
                    responce.Edit(string.Format("{0} Timer is running! Time left: {1}", args.Emoji, UptimeExecutor.GetFormatedTime(remining)));
                } else {
                    responce.Edit(string.Format("{0} Timer finished {1}", args.Emoji, DateTime.Now.ToString(BlameExecutor.DatePatten)));
                }
            };

            return ExecutionResult.Success;
        }

        // Utils

        public static int GetSecondsFromInput(string input) {
            int numeric;
            
            if(int.TryParse(input.Substring(0, input.Length - 1), out numeric)) {
                switch(input.Substring(input.Length - 1)) {
                    case "s":
                        return numeric;
                    case "m":
                        return numeric * 60;
                    case "h":
                        return numeric * 60 * 60;
                }

            }

            return -1;
        }

        // Countdown

        public class Countdown {

            public class StateChangedEventArgs : EventArgs {

                public double   State       { private set; get; }
                public string   Emoji       { private set; get; }
                public TimeSpan Remining    { private set; get; }

                public StateChangedEventArgs(Countdown countdown) {
                    this.State = countdown.GetState();
                    this.Emoji = string.Format(":clock{0}:", this.State);
                    this.Remining = countdown.GetReminingTime();
                }

            }

            private Timer Timer;
            private TimeSpan Time;
            private double ElapsedSeconds;
            private double LastState;

            public event EventHandler<StateChangedEventArgs> OnStateChanged;

            public Countdown(TimeSpan time) {
                ElapsedSeconds = 0;
                Time = time;
                Timer = new Timer(1000);
                Timer.Elapsed += OnTimerElapsed;
                Timer.Start();
            }

            private void OnTimerElapsed(object sender, ElapsedEventArgs e) {
                if(ElapsedSeconds < Time.TotalSeconds) {
                    ElapsedSeconds++;

                    double state = GetState();
                    if(state != LastState) {
                        LastState = state;
                        OnStateChanged(this, new StateChangedEventArgs(this));
                    }
                } else {
                    Timer.Stop();
                    Timer.Dispose();
                    OnStateChanged(this, new StateChangedEventArgs(this));
                }
            }

            public double GetState() {
                return Math.Round(12 / 100F * (ElapsedSeconds / Time.TotalSeconds * 100F));
            }

            public TimeSpan GetReminingTime() {
                return TimeSpan.FromSeconds(Time.TotalSeconds - ElapsedSeconds);
            }

        }

    }

}
