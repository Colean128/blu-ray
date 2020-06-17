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

using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace Bot.Structures
{
    public class IMDb
    {
        public enum EntryType
        {
            Unknown,
            Movie,
            Series,
            Episode
        }

        private const string omdbURL = "http://www.omdbapi.com";
        internal static string apiKey;

        public class SearchEntry
        {
            [JsonProperty("Title")]
            public string Title { get; private set; }

            [JsonProperty("Year")]
            public string Year { get; private set; }

            [JsonProperty("imdbID")]
            public string ID { get; private set; }

            public string URL { get => "https://www.imdb.com/title/" + ID; }

            [JsonProperty("Type")]
            private string type;

            public EntryType Type
            {
                get
                {
                    switch (type)
                    {
                    case "movie":
                        return EntryType.Movie;

                    case "series":
                        return EntryType.Series;

                    case "episode":
                        return EntryType.Episode;

                    default:
                        return EntryType.Unknown;
                    }
                }
            }

            [JsonProperty("Poster")]
            public string Poster { get; internal set; }
        }

        [JsonProperty("Search")]
        public List<SearchEntry> Entries { get; internal set; }

        [JsonProperty("totalResults")]
        private string results;

        public int Results { get => int.Parse(results); }

        [JsonProperty("Response")]
        private string response;

        public bool Responded
        {
            get
            {
                if (response == "True") return true;
                return false;
            }
        }

        public IMDb() { }

        public static async Task<IMDb> GetAsync(string query)
        {
            HttpClient client               = new HttpClient();
            HttpResponseMessage response    = await client.GetAsync(omdbURL + $"/?apikey={apiKey}&s={HttpUtility.UrlEncode(query)}");
            string content                  = await response.Content.ReadAsStringAsync();

            response.Dispose();
            client.Dispose();

            return JsonConvert.DeserializeObject<IMDb>(content);
        }
    }
}
