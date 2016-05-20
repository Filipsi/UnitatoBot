using RestSharp.Extensions.MonoHttp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UnitatoBot.Utilities.Http {

    public partial class Http {

        private static Thread RequesterThread;
        private static LinkedList<Request> QueuedRequests;

        static Http() {
            QueuedRequests = new LinkedList<Request>();
            RequesterThread = new Thread(new ThreadStart(HandleRequesterThread));
            RequesterThread.Start();
        }

        private static void HandleRequesterThread() {
            while(true) {
                if(QueuedRequests.Count < 1) { Thread.Sleep(250); continue; }

                for(int i = 0; i < QueuedRequests.Count; i++) {
                    Request request = QueuedRequests.ElementAt(i);
                    request.MakeRequest();
                    QueuedRequests.Remove(request);
                }
            }
        }

        public static Request QueryRequest(HttpMethod method, string url, params string[] args) {
            Request request = new Request(method, url, args);
            QueuedRequests.AddLast(request);
            return request;
        }

        public static string PerformHttpRequest(HttpMethod method, string Url, string[] postdata) {
            string result = string.Empty;
            string data = string.Empty;

            if(postdata.Length % 2 != 0) {
                throw new Exception("Parameter postdata has to contain pairs of data!");
            }

            for(int i = 0; i < postdata.Length; i += 2) {
                data += string.Format("&{0}={1}", postdata[i], postdata[i + 1]);
            }

            data = data.Remove(0, 1);

            byte[] bytesarr = Encoding.UTF8.GetBytes(data);
            WebRequest request = method == HttpMethod.POST ? WebRequest.Create(Url) : WebRequest.Create(Url + "?" + data);

            request.Method = method.ToString();

            if(method == HttpMethod.POST) {
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = bytesarr.Length;

                System.IO.Stream streamwriter = request.GetRequestStream();
                streamwriter.Write(bytesarr, 0, bytesarr.Length);
                streamwriter.Close();

                WebResponse response = request.GetResponse();
                streamwriter = response.GetResponseStream();

                System.IO.StreamReader streamread = new System.IO.StreamReader(streamwriter);
                result = streamread.ReadToEnd();
                streamread.Close();
            } else {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                StreamReader resStream = new StreamReader(response.GetResponseStream());
                result = resStream.ReadToEnd();
            }

            return result;
        }

    }

}
