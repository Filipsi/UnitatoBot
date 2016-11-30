using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitatoBot.Util.FilipsiNetApi {

    static class FilipsiNetApi {

        private static RestClient _api = new RestClient("http://api.filipsi.net/");

        private static void MakeRequest(string path, Action<IRestResponse> callback) {
            _api.ExecuteAsync(new RestRequest(path, Method.GET), callback);
        }

        // Public calls

        public static void GetFaggotPoints(string username, Action<int> callback) {
            MakeRequest("faggotpoints/" + username, responce => {
                int points; if(int.TryParse(responce.Content, out points) && points > -1) callback(points);
            });
        }

        public static void GetFaggotPoints(Action<JObject> callback) {
            MakeRequest("faggotpoints", responce => {
                callback(JObject.Parse(responce.Content));
            });
        }

    }

}
