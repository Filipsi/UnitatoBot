using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitatoBot.Utilities.Http {

    public class Request {

        public event EventHandler<ReqestEventArgs> OnReqestComplete;

        public HttpMethod Method { private set; get; }
        public string Url { private set; get; }
        public string[] Args { private set; get; }
        public bool WasRequestMade { private set; get; }

        public Request(HttpMethod method, string url, params string[] data) {
            this.WasRequestMade = false;
            this.Method = method;
            this.Url = url;
            this.Args = data;
        }

        public void MakeRequest() {
            if(WasRequestMade) return;

            string responce = Http.PerformHttpRequest(this.Method, this.Url, this.Args);
            OnReqestComplete(this, new ReqestEventArgs(responce));
            WasRequestMade = true;
        }

    }

}
