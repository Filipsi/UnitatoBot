using System;
using Newtonsoft.Json;

namespace UnitatoBot.Util.UsageManager {

    internal partial class UsageManager {

        [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
        public class Usage {

            [JsonProperty(IsReference = true)]
            public UsageManager Manager {
                private set;
                get;
            }

            [JsonProperty]
            public int Uses {
                private set;
                get;
            }

            [JsonProperty]
            public DateTime LastReset;

            public bool ShouldSave {
               get {
                    return Uses != Manager.MaximumUses && !ShouldReset;
               }
            }

            public bool ShouldReset {
                get {
                    return (DateTime.Now - LastReset) >= Manager.ResetPeriod;
                }
            }

            public bool CanBeUsed {
                get {
                    if(ShouldReset) Reset();
                    return Uses > 0;
                }
            }

            public TimeSpan TimeUntilReset {
                get { return Manager.ResetPeriod - (DateTime.Now - LastReset); }
            }

            public Usage(UsageManager manager) {
                Manager = manager;
            }

            private void Reset() {
                Uses = Manager.MaximumUses;
                LastReset = DateTime.Now;
            }

            public bool UseOnce() {
                if(ShouldReset)
                    Reset();

                if (!CanBeUsed)
                    return false;

                Uses--;
                Manager.Save();
                return true;
            }

        }

    }

}
