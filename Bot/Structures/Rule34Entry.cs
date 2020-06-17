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

using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Bot.Structures
{
    public class Rule34Entry
    {
        [JsonProperty("score")]
        public string Score { get; private set; }

        [JsonProperty("file_url")]
        public string URL { get; private set; }

        [JsonProperty("rating")]
        public string Rating { get; private set; }

        [JsonProperty("id")]
        public string ID { get; private set; }

        [JsonProperty("source")]
        public string Source { get; private set; }

        [JsonProperty("type")]
        public string Type { get; private set; }

        [JsonProperty("creator_url")]
        public string CreatorURL { get; private set; }

        [JsonProperty("preview_url")]
        public string PreviewURL { get; private set; }

        public string PostURL { get => "https://rule34.xxx/index.php?page=post&s=view&id=" + ID; }

        public Rule34Entry() { }

        public static async Task<Rule34Entry[]> GetAsync()
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync("https://r34-json-api.herokuapp.com/posts");
            string content = await response.Content.ReadAsStringAsync();

            response.Dispose();
            client.Dispose();

            return JsonConvert.DeserializeObject<Rule34Entry[]>(content);
        }

        public static async Task<Rule34Entry[]> GetAsync(string query)
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync("https://r34-json-api.herokuapp.com/posts?tags=" +query);
            string content = await response.Content.ReadAsStringAsync();

            response.Dispose();
            client.Dispose();

            return JsonConvert.DeserializeObject<Rule34Entry[]>(content);
        }


    }
}