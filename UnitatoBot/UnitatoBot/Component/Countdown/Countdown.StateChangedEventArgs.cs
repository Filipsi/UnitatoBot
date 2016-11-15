using System;

namespace UnitatoBot.Component.Countdown {

    public partial class Countdown {

        public class StateChangedEventArgs : EventArgs {

            public double Stage { private set; get; }
            public string Icon { private set; get; }
            public TimeSpan Remining { private set; get; }

            public StateChangedEventArgs(Countdown countdown) {
                this.Stage = countdown.Stage;
                this.Icon = string.Format(":clock{0}:", this.Stage);
                this.Remining = countdown.Remining;
            }

        }

    }

}
