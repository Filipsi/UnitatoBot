using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitatoBot.Connector {

    internal interface IAudioCapability {

        void SendAudio(string destination, string file);

    }

}
