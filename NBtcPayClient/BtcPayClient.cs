using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace NBtcPayClient
{
    public class BtcPayClient : IDisposable
    {
        private HttpClient _httpClient;

        public BtcPayClient(string hostUrl)
        {
            _httpClient = new HttpClient {BaseAddress = new Uri(hostUrl)};
        }
        
        public BtcPayClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task AuthenticateViaPasswordFlow(string username, string password, string clientId = null)
        {
            var response = await _httpClient.PostAsync("connect/token",
                new FormUrlEncodedContent(
                    new Dictionary<string, string>()
                    {
                        {"grant_type", "password"},
                        {"username", username},
                        {"password", password},
                        {"client_id", clientId}
                    }));

            response.EnsureSuccessStatusCode();
            var parsedResponse = JObject.Parse(await response.Content.ReadAsStringAsync());
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}