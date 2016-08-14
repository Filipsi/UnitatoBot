using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitatoBot.Component.Countdown {

    public partial class Countdown {

        public class StateChangedEventArgs : EventArgs {

            public double State { private set; get; }
            public string Emoji { private set; get; }
            public TimeSpan Remining { private set; get; }

            public StateChangedEventArgs(Countdown countdown) {
                this.State = countdown.GetState();
                this.Emoji = string.Format(":clock{0}:", this.State);
                this.Remining = countdown.GetReminingTime();
            }

        }

    }

}
