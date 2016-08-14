using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace UnitatoBot.Component.Countdown {

    public partial class Countdown {

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
