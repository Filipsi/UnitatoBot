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
            return "Create a timer that will count down from number given as argument to zero in seconds";
        }

        public ExecutionResult CanExecute(CommandContext context) {
            int sec;
            return context.HasArguments && context.Args.Count() == 1 && int.TryParse(context.Args[0], out sec) ? ExecutionResult.Success : ExecutionResult.Fail;
        }

        public ExecutionResult Execute(CommandContext context) {
            Countdown countdown = new Countdown(TimeSpan.FromSeconds(int.Parse(context.Args[0])));
            ConnectionMessage responce = context.ResponseBuilder.With("Timer set to").With(int.Parse(context.Args[0])).With("seconds.").Send();

            countdown.OnStateChanged += (sender, args) => {
                TimeSpan remining = args.Remining;
                if(remining.TotalSeconds > 0) {
                    responce.Edit(string.Format("{0} Timer running! Time left: {1}", args.Emoji, UptimeExecutor.GetFormatedTime(remining)));
                } else {
                    responce.Edit(string.Format("{0} Timer finished {1}", args.Emoji, DateTime.Now.ToString(BlameExecutor.DatePatten)));
                }
            };

            return ExecutionResult.Success;
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
