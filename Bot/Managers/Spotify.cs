using DSharpPlus;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
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

        public class SearchResponse
        {
            public class ArtistElement
            {
                [JsonProperty("id")]
                public string ID { get; private set; }

                [JsonProperty("name")]
                public string Name { get; private set; }

                public string URL { get => "https://open.spotify.com/artist/" + ID; }
            }

            public class AlbumElement
            {
                public class ImageElement
                {
                    [JsonProperty("height")]
                    public int Height { get; private set; }

                    [JsonProperty("width")]
                    public int Width { get; private set; }

                    [JsonProperty("url")]
                    public string URL { get; private set; }
                }

                [JsonProperty("artists")]
                public ArtistElement[] Artists;

                [JsonProperty("id")]
                public string ID { get; private set; }

                [JsonProperty("images")]
                public ImageElement[] Images { get; private set; }

                [JsonProperty("name")]
                public string Name { get; private set; }

                [JsonProperty("release_date")]
                public string ReleaseDate { get; private set; }

                [JsonProperty("total_tracks")]
                public int TotalTracks { get; private set; }

                public string URL { get => "https://open.spotify.com/album/" + ID; }
            }
        }

        private static string base64Encode(string plainText) => Convert.ToBase64String(Encoding.UTF8.GetBytes(plainText));

        private static HttpClient client;
        private static DebugLogger logger;

        private static string clientID;
        private static string clientSecret;

        private static AuthorizationResponse currentAuth;

        private static Task authorizeRefreshThread()
        {
            while (true)
            {
                try
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
                catch (Exception ex)
                {
                    logger.LogMessage(LogLevel.Error, "Spotify", "The manager failed to authorize! Spotify will be unavailable.", DateTime.Now, ex);
                    return Task.CompletedTask;
                }
            }
        }

        public static async Task AuthorizeAsync(string id, string secret, DebugLogger log)
        {
            client = new HttpClient();
            logger = log;

            clientID = id;
            clientSecret = secret;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://accounts.spotify.com/api/token?grant_type=client_credentials");
                request.Headers.Add("Authorization", $"Basic {base64Encode(clientID + ":" + clientSecret)}");
                request.ContentType = "application/x-www-form-urlencoded";
                request.Method = "POST";
                request.ContentLength = 0;

                WebResponse message = await request.GetResponseAsync();
                currentAuth = JsonConvert.DeserializeObject<AuthorizationResponse>(await new System.IO.StreamReader(message.GetResponseStream()).ReadToEndAsync());
            }
            catch (Exception ex)
            {
                logger.LogMessage(LogLevel.Error, "Spotify", "The manager failed to authorize! Spotify will be unavailable.", DateTime.Now, ex);
                return;
            }

            new Thread(new ThreadStart(() => authorizeRefreshThread())).Start();
        }

        public static async Task<SearchResponse> SearchAsync(string query)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://api.spotify.com/v1/search?q=&type=track,album&limit=1");
            request.Headers.Add("Authorization", $"Bearer {currentAuth.AccessToken}");
            request.ContentType = "application/x-www-form-urlencoded";
            request.Method = "GET";
            request.ContentLength = 0;

            WebResponse message = await request.GetResponseAsync();

            return JsonConvert.DeserializeObject<SearchResponse>(await new System.IO.StreamReader(message.GetResponseStream()).ReadToEndAsync(), new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
        }
    }
}