// Blu-Ray Discord Bot
//
// Copyright(C) 2020 Colean, Apfel
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.

using DSharpPlus;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

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

            public class TrackElement
            {
                [JsonProperty("album")]
                public AlbumElement Album { get; private set; }

                [JsonProperty("artists")]
                public ArtistElement[] Artists { get; private set; }

                [JsonProperty("duration_ms")]
                public int Duration { get; private set; }

                [JsonProperty("explicit")]
                public bool Expicit { get; private set; }

                [JsonProperty("id")]
                public string ID { get; private set; }

                [JsonProperty("name")]
                public string Name { get; private set; }

                [JsonProperty("popularity")]
                public string Popularity { get; private set; }

                [JsonProperty("track_number")]
                public int TrackNumber { get; private set; }

                public string URL { get => "https://open.spotify.com/track/" + ID; }
            }

            public class AlbumsElement
            {
                [JsonProperty("items")]
                public AlbumElement[] Items { get; private set; }
            }

            public class TracksElement
            {
                [JsonProperty("items")]
                public TrackElement[] Items { get; private set; }
            }

            [JsonProperty("albums")]
            public AlbumsElement Albums { get; private set; }

            [JsonProperty("tracks")]
            public TracksElement Tracks { get; private set; }
        }

        private static string base64Encode(string plainText) => Convert.ToBase64String(Encoding.UTF8.GetBytes(plainText));

        private static HttpClient client;
        private static DebugLogger logger;

        private static string clientID;
        private static string clientSecret;

        private static AuthorizationResponse currentAuth;

        private static async Task authorizeRefreshThread()
        {
            while (true)
            {
                try
                {
                    await Task.Delay(currentAuth.Expires * 1000);

                    HttpWebRequest request  = (HttpWebRequest)WebRequest.Create("https://accounts.spotify.com/api/token?grant_type=client_credentials");
                    request.Headers.Add("Authorization", $"Basic {base64Encode(clientID + ":" + clientSecret)}");
                    request.ContentType     = "application/x-www-form-urlencoded";
                    request.Method          = "POST";
                    request.ContentLength   = 0;

                    WebResponse message     = await request.GetResponseAsync();
                    currentAuth             = JsonConvert.DeserializeObject<AuthorizationResponse>(await new StreamReader(message.GetResponseStream()).ReadToEndAsync());
                    logger.LogMessage(LogLevel.Debug, "Spotify", "Re-authorized successfully.", DateTime.Now);

                    message.Dispose();
                }
                catch (Exception ex)
                {
                    logger.LogMessage(LogLevel.Error, "Spotify", "The manager failed to authorize! Spotify will be unavailable for a few moments.", DateTime.Now, ex);
                    continue;
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
                HttpWebRequest request  = (HttpWebRequest)WebRequest.Create("https://accounts.spotify.com/api/token?grant_type=client_credentials");
                request.Headers.Add("Authorization", $"Basic {base64Encode(clientID + ":" + clientSecret)}");
                request.ContentType     = "application/x-www-form-urlencoded";
                request.Method          = "POST";
                request.ContentLength   = 0;

                WebResponse message     = await request.GetResponseAsync();
                currentAuth             = JsonConvert.DeserializeObject<AuthorizationResponse>(await new System.IO.StreamReader(message.GetResponseStream()).ReadToEndAsync());

                message.Dispose();
            }
            catch (Exception ex)
            {
                logger.LogMessage(LogLevel.Error, "Spotify", "The manager failed to authorize! Spotify will be unavailable.", DateTime.Now, ex);
                return;
            }

            new Thread(new ThreadStart(async () => await authorizeRefreshThread())).Start();
        }

        public static async Task<SearchResponse> SearchAsync(string query)
        {
            HttpWebRequest request  = (HttpWebRequest)WebRequest.Create($"https://api.spotify.com/v1/search?q={HttpUtility.UrlEncode(query)}&type=track,album&limit=1");
            request.Headers.Add("Authorization", $"Bearer {currentAuth.AccessToken}");
            request.ContentType     = "application/x-www-form-urlencoded";
            request.Method          = "GET";
            request.ContentLength   = 0;

            WebResponse message     = await request.GetResponseAsync();
            SearchResponse response =  JsonConvert.DeserializeObject<SearchResponse>(await new System.IO.StreamReader(message.GetResponseStream()).ReadToEndAsync(), new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

            message.Dispose();

            return response;
        }
    }
}