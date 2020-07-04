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
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
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
        public class EmojiConfiguration
        {
            [JsonProperty("success", NullValueHandling = NullValueHandling.Include)]
            public ulong Success { get; private set; }

            [JsonProperty("warning", NullValueHandling = NullValueHandling.Include)]
            public ulong Warning { get; private set; }

            [JsonProperty("error", NullValueHandling = NullValueHandling.Include)]
            public ulong Error { get; private set; }

            [JsonProperty("online", NullValueHandling = NullValueHandling.Include)]
            public ulong Online { get; private set; }

            [JsonProperty("idle", NullValueHandling = NullValueHandling.Include)]
            public ulong Idle { get; private set; }

            [JsonProperty("dnd", NullValueHandling = NullValueHandling.Include)]
            public ulong DoNotDisturb { get; private set; }

            [JsonProperty("offline", NullValueHandling = NullValueHandling.Include)]
            public ulong Offline { get; private set; }
        }

        public class GoogleConfiguration
        {
            [JsonProperty("key", NullValueHandling = NullValueHandling.Include)]
            public string Key { get; private set; }

            [JsonProperty("cx", NullValueHandling = NullValueHandling.Include)]
            public string Cx { get; private set; }
        }

        public class SpotifyConfiguration
        {
            [JsonProperty("id", NullValueHandling = NullValueHandling.Include)]
            public string ID { get; private set; }

            [JsonProperty("secret", NullValueHandling = NullValueHandling.Include)]
            public string Secret { get; private set; }
        }

        public class StatusConfiguration
        {
            [JsonProperty("name", NullValueHandling = NullValueHandling.Include)]
            public string Name { get; private set; }

            [JsonProperty("Type", NullValueHandling = NullValueHandling.Include)]
            public ActivityType Type { get; private set; }
        }

        [JsonProperty("token", NullValueHandling = NullValueHandling.Include)]
        public string Token { get; private set; }

        [JsonProperty("loglevel", NullValueHandling = NullValueHandling.Include)]
        public string LogLevel { get; private set; }

        [JsonProperty("prefixes", NullValueHandling = NullValueHandling.Include)]
        public string[] Prefixes { get; private set; }

        [JsonProperty("status", NullValueHandling = NullValueHandling.Include)]
        public StatusConfiguration Status { get; private set; }

        [JsonProperty("emojis", NullValueHandling = NullValueHandling.Ignore)]
        public EmojiConfiguration Emojis;

        [JsonProperty("support_guild", NullValueHandling = NullValueHandling.Include)]
        public ulong SupportId { get; private set; }

        [JsonProperty("google", NullValueHandling = NullValueHandling.Include)]
        public GoogleConfiguration Google;

        [JsonProperty("omdb", NullValueHandling = NullValueHandling.Include)]
        public string OMDb { get; internal set; }

        [JsonProperty("spotify", NullValueHandling = NullValueHandling.Include)]
        public SpotifyConfiguration Spotify { get; private set; }

        [JsonProperty("steam", NullValueHandling = NullValueHandling.Include)]
        public string Steam { get; private set; }

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
            Emojis      = config.Emojis;
            Status      = config.Status;
            SupportId   = config.SupportId;
            Google      = config.Google;
            OMDb        = config.OMDb;
            Spotify     = config.Spotify;
            Steam       = config.Steam;
        }
    }
}
