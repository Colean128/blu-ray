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
// along with this program. If not, see <http://www.gnu.org/licenses/>.

using Bot.Managers;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Threading.Tasks;

namespace Bot.Commands
{
    public class Search : BaseCommandModule
    {
        [Command("spotify"), Description("Searches for a track or an album on Spotify.")]
        public async Task SpotifyAsync(CommandContext context, [RemainingText, Description("Search query to search for.")] string query = null)
        {
            if (query == null)
            {
                await context.RespondAsync("Provide a query to search for.");
                return;
            }

            Spotify.SearchResponse response = null;
            try { response = await Spotify.SearchAsync(query); }
            catch (Exception ex)
            {
                context.Client.DebugLogger.LogMessage(LogLevel.Error, "Commands - Spotify", "Search failed for \"{query}\".", DateTime.Now, ex);
                await context.RespondAsync("Failed to find anything.");
                return;
            }

            if (response == null || (response.Albums == null && response.Tracks == null))
            {
                await context.RespondAsync("Failed to find anything.");
                return;
            }

            string artists = "";
            if (response.Tracks != null)
            {
                Spotify.SearchResponse.TrackElement track = response.Tracks.Items[0];

                foreach (Spotify.SearchResponse.ArtistElement artist in track.Artists) artists += $"**{artist.Name}**, ";
                artists = artists.Substring(0, artists.LastIndexOf(", "));
                
                TimeSpan span = TimeSpan.FromMilliseconds(track.Duration);

                await context.RespondAsync(embed: new DiscordEmbedBuilder
                {
                    Color = new DiscordColor("1DB954"),
                    Description = $"**{track.Name}** by {artists}.\nLength: **{string.Format("{0:D2}:{1:D2}", span.Minutes, span.Seconds)}**.",
                    Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail {
                        Url = track.Album.Images[0].URL,
                    },                    
                    Title = "Search result",
                    Url = track.URL
                }.Build());

                return;
            }

            Spotify.SearchResponse.AlbumElement album = response.Albums.Items[0];

            foreach (Spotify.SearchResponse.ArtistElement artist in album.Artists) artists += $"**{artist.Name}**, ";
            artists = artists.Substring(0, artists.LastIndexOf(", "));

            await context.RespondAsync(embed: new DiscordEmbedBuilder
            {
                Color = new DiscordColor("1DB954"),
                Description = $"Album **{album.Name}** by **{artists}**",
                Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail
                {
                    Url = album.Images[0].URL,
                },
                Title = "Search result",
                Url = album.URL
            }.Build());
        }
    }
}
