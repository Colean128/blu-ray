// Blu-Ray Discord Bot
//
// Copyright © 2020, The Blu-Ray authors 
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

using DSharpPlus.Entities;
using Newtonsoft.Json;
using System.IO;

namespace Bot.Structures
{
    public class Configuration
    {
        public class GoogleConfiguration
        {
            [JsonProperty("key")]
            public string Key { get; private set; }

            [JsonProperty("cx")]
            public string Cx { get; private set; }
        }

        public class SpotifyConfiguration
        {
            [JsonProperty("id")]
            public string ID { get; private set; }

            [JsonProperty("secret")]
            public string Secret { get; private set; }
        }

        public class StatusConfiguration
        {
            [JsonProperty("name")]
            public string Name { get; private set; }

            [JsonProperty("Type")]
            public ActivityType Type { get; private set; }
        }

        [JsonProperty("token")]
        public string Token { get; private set; }

        [JsonProperty("loglevel")]
        public string LogLevel { get; private set; }

        [JsonProperty("prefixes")]
        public string[] Prefixes { get; private set; }

        [JsonProperty("status")]
        public StatusConfiguration Status { get; private set; }

        [JsonProperty("support_guild")]
        public ulong SupportId { get; private set; }

        [JsonProperty("google")]
        public GoogleConfiguration Google;

        [JsonProperty("omdb")]
        public string OMDb { get; internal set; }

        [JsonProperty("spotify")]
        public SpotifyConfiguration Spotify { get; private set; }

        public Configuration() { }

        public Configuration(string path)
        {
            FileStream file     = File.OpenRead(path);
            StreamReader reader = new StreamReader(file);

            Configuration config = JsonConvert.DeserializeObject<Configuration>(reader.ReadToEnd());

            reader.Close();
            file.Close();

            Token       = config.Token;
            LogLevel    = config.LogLevel;
            Prefixes    = config.Prefixes;
            Status      = config.Status;
            SupportId   = config.SupportId;
            Google      = config.Google;
            OMDb        = config.OMDb;
            Spotify     = config.Spotify;
        }
    }
}
