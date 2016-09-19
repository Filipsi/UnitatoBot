using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitatoBot.Bridge {

    internal interface IAudioCapability {

        bool SendAudio(string destination, string file);

    }

}
