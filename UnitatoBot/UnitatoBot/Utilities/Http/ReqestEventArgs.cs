using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitatoBot.Utilities.Http {

    public class ReqestEventArgs : EventArgs {

        public string Responce { private set; get; }

        public ReqestEventArgs(string responce) {
            this.Responce = responce;
        }

    }

}
