using Newtonsoft.Json.Linq;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitatoBot.Configuration;

namespace UnitatoBot.Util.WebApi {

    static class WebApi {

        private static RestClient _api = new RestClient {
            BaseUrl = new Uri(Config.Settings.ApiUrl)
        };

        private static void GetRequest(string path, Action<IRestResponse> callback) {
            _api.ExecuteAsync(new RestRequest(path, Method.GET), callback);
        }

        // Public calls

        public static void GetFaggotPoints(string username, Action<int> callback) {
            GetRequest("faggotpoints/" + username, responce => {
                int points;
                if(int.TryParse(responce.Content, out points) && points > -1)
                    callback(points);
            });
        }

        public static void GetFaggotPoints(Action<JObject> callback) {
            GetRequest("faggotpoints", responce => {
                callback(JObject.Parse(responce.Content));
            });
        }

        public static void SetFaggotPoints(string username, int amount, Action<bool> callback) {
            _api.ExecuteAsync(
                new RestRequest("faggotpoints/" + username, Method.POST)
                    .AddParameter("key", Config.Settings.ApiKey)
                    .AddParameter("amount", amount),
                
                response => {
                    bool result = false;
                    bool.TryParse(response.Content, out result);
                    callback(result);
                }
            );
        }

    }

}
