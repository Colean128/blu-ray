using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Bot.Managers
{
    public class Spotify
    {
        private class AuthorizationResponse
        {
            [JsonProperty("access_token")]
            public string AccessToken { get; private set; }

            [JsonProperty("expires_in")]
            public int Expires { get; private set; }
        }

        private static string base64Encode(string plainText) => Convert.ToBase64String(Encoding.UTF8.GetBytes(plainText));

        private static HttpClient client;

        private static string clientID;
        private static string clientSecret;

        private static AuthorizationResponse currentAuth;

        private static Task authorizeRefreshThread()
        {
            while (true)
            {
                Task.Delay(currentAuth.Expires * 1000);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://accounts.spotify.com/api/token?grant_type=client_credentials");
                request.Headers.Add("Authorization", $"Basic {base64Encode(clientID + ":" + clientSecret)}");
                request.ContentType = "application/x-www-form-urlencoded";
                request.Method = "POST";
                request.ContentLength = 0;

                WebResponse message = request.GetResponse();
                currentAuth = JsonConvert.DeserializeObject<AuthorizationResponse>(new System.IO.StreamReader(message.GetResponseStream()).ReadToEnd());
            }
        }

        public static async Task AuthorizeAsync(string id, string secret)
        {
            client = new HttpClient();

            clientID = id;
            clientSecret = secret;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://accounts.spotify.com/api/token?grant_type=client_credentials");
            request.Headers.Add("Authorization", $"Basic {base64Encode(clientID + ":" + clientSecret)}");
            request.ContentType = "application/x-www-form-urlencoded";
            request.Method = "POST";
            request.ContentLength = 0;

            WebResponse message = await request.GetResponseAsync();
            currentAuth = JsonConvert.DeserializeObject<AuthorizationResponse>(await new System.IO.StreamReader(message.GetResponseStream()).ReadToEndAsync());
            
            new Thread(new ThreadStart(() => authorizeRefreshThread())).Start();
        }
    }
}