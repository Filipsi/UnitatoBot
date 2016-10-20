using Newtonsoft.Json;
using System;

namespace UnitatoBot.Util.UsageManager {

    internal partial class UsageManager {

        public class Usage {

            public int Max { private set; get; }
            public int Count { private set; get; }
            public TimeSpan ResetPeriod { private set; get; }

            [JsonProperty]
            private DateTime LastReset;

            [JsonIgnore]
            public bool ShouldSave {
               get { return Count != Max && !ShouldReset; }
            }

            [JsonIgnore]
            public bool ShouldReset {
                get { return (DateTime.Now - LastReset) >= ResetPeriod; }
            }

            [JsonIgnore]
            public bool CanBeUsed {
                get {
                    if(ShouldReset) Reset();
                    return Count > 0;
                }
            }

            [JsonIgnore]
            public TimeSpan TimeUntilReset {
                get { return ResetPeriod - (DateTime.Now - LastReset); }
            }

            [JsonConstructor]
            private Usage(int max, int count, TimeSpan resetPeriod, DateTime lastReset) {
                Max = max;
                Count = count;
                ResetPeriod = resetPeriod;
                LastReset = lastReset;
            }

            public Usage(int max, TimeSpan resetPeriod) {
                Max = max;
                Count = max;
                ResetPeriod = resetPeriod;
                LastReset = DateTime.Today;
            }

            private void Reset() {
                Count = Max;
                LastReset = DateTime.Now;
            }

            public bool UseOnce() {
                if(ShouldReset)
                    Reset();

                if(CanBeUsed) {
                    Count--;
                    return true;
                }

                return false;
            }

        }

    }

}
