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

        [JsonProperty("tags")]
        public string[] Tags { get; private set; }

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

        public Rule34Entry() { }

        public static async Task<Rule34Entry[]> GetAsync()
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync("https://r34-json-api.herokuapp.com/posts");
            string content = await response.Content.ReadAsStringAsync();

            client.Dispose();

            return JsonConvert.DeserializeObject<Rule34Entry[]>(content);
        }

        public static async Task<Rule34Entry[]> GetAsync(string query)
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync("https://r34-json-api.herokuapp.com/posts?tags=" +query);
            string content = await response.Content.ReadAsStringAsync();

            client.Dispose();

            return JsonConvert.DeserializeObject<Rule34Entry[]>(content);
        }


    }
}