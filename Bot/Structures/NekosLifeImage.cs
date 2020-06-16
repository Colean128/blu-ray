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
using System.Net.Http;
using System.Threading.Tasks;

namespace Bot.Structures
{
    public class NekosLifeImage
    {
        private const string nekosLifeURL = "https://nekos.life/api/v2/img/";

        public enum Endpoint
        {
            Hug,
            Kiss,
            Meow,
            Pat,
            Slap,
            Woof
        }

        [JsonProperty("url")]
        public string URL { get; private set; }

        public NekosLifeImage() { }

        public static async Task<string> GetAsync(Endpoint endpoint)
        {
            string end = "";
            switch (endpoint)
            {
                case Endpoint.Hug:
                    end = "hug";
                    break;

                case Endpoint.Kiss:
                    end = "kiss";
                    break;

                case Endpoint.Meow:
                    end = "meow";
                    break;

                case Endpoint.Pat:
                    end = "pat";
                    break;

                case Endpoint.Slap:
                    end = "slap";
                    break;

                case Endpoint.Woof:
                    end = "woof";
                    break;
            }

            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(nekosLifeURL + end);
            string content = await response.Content.ReadAsStringAsync();

            client.Dispose();

            return JsonConvert.DeserializeObject<NekosLifeImage>(content).URL;
        }
    }
}
