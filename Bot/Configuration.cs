// Blu-Ray Discord Bot
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
// along with this program.If not, see<http://www.gnu.org/licenses/>.

using Newtonsoft.Json;
using System.IO;

namespace Bot
{
    internal class Configuration
    {
        [JsonProperty("token")]
        public string Token { get; private set; }

        [JsonProperty("prefixes")]
        public string[] Prefixes { get; private set; }

        [JsonProperty("spotify")]
        public string Spotify { get; private set; }

        public Configuration() { }

        public Configuration(string path)
        {
            FileStream file = File.OpenRead(path);
            StreamReader reader = new StreamReader(file);

            Configuration config = JsonConvert.DeserializeObject<Configuration>(reader.ReadToEnd());

            reader.Close();
            file.Close();

            Token = config.Token;
            Prefixes = config.Prefixes;
            Spotify = config.Spotify;
        }
    }
}
