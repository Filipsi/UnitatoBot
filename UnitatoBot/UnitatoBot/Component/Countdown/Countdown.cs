using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Timers;

namespace Unitato.Component.Countdown {

    public partial class Countdown {

        private Timer       Timer;
        private TimeSpan    Length;
        private double      ElapsedSeconds;
        private double      LastStage;

        public double Stage {
            get {
                return Math.Round(12 / 100F * (ElapsedSeconds / Length.TotalSeconds * 100F));
            }
        }

        public TimeSpan Remining  {
            get {
                return TimeSpan.FromSeconds(Length.TotalSeconds - ElapsedSeconds);
            }
        }

        public event EventHandler<StateChangedEventArgs> OnStateChanged;

        public Countdown(TimeSpan length) {
            ElapsedSeconds = 0;
            Length = length;
            Timer = new Timer(1000);
            Timer.Elapsed += OnTimerElapsed;
            Timer.Start();
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e) {
            if(ElapsedSeconds < Length.TotalSeconds) {
                ElapsedSeconds++;

                double stage = Stage;
                if(stage != LastStage) {
                    LastStage = stage;
                    OnStateChanged(this, new StateChangedEventArgs(this));
                }
            } else {
                Timer.Stop();
                Timer.Dispose();
                OnStateChanged(this, new StateChangedEventArgs(this));
            }
        }

    }

}
