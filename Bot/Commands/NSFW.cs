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

using System;
using System.Text;
using System.Threading.Tasks;
using Bot.Structures;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

namespace Bot.Commands
{
    public class NSFW : BaseCommandModule
    {
        [Command("rule34"), Description("Searches for content on Rule34."), Aliases("r34"), RequireNsfw]
        public async Task Rule34Async(CommandContext context, [RemainingText, Description("Search query.")] string query = null)
        {
            Rule34Entry[] entries;
            Rule34Entry entry;
            DiscordEmbedBuilder builder = new DiscordEmbedBuilder();

            if (query == null)
            {
                entries = await Rule34Entry.GetAsync();
                if (entries.Length == 0)
                {
                    await context.RespondAsync("Failed to find random posts.");
                    return;
                }

                entry = entries[new Random().Next(0, entries.Length - 1)];

                builder.WithTitle("Random post")
                    .WithUrl(entry.PostURL)
                    .AddField("Score", entry.Score).AddField("Rating", entry.Rating, true)
                    .AddField("Creator", $"[URL]({entry.CreatorURL})");

                if (entry.Type == "video") builder.WithThumbnail(entry.PreviewURL).Description += $"\n**[Video URL]({entry.URL})**";
                else builder.WithImageUrl(entry.URL);

                await context.RespondAsync(embed: builder.Build());
                return;
            }

            entries = await Rule34Entry.GetAsync(query);
            if (entries.Length == 0)
            {
                await context.RespondAsync($"Failed to find any posts for the following query:\n```\n{query}\n```");
                return;
            }

            entry = entries[new Random().Next(0, entries.Length - 1)];

            builder.WithTitle("Searched post")
                .WithDescription($"Query:\n```\n{query}\n```")
                .WithUrl(entry.PostURL)
                .AddField("Score", entry.Score).AddField("Rating", entry.Rating, true)
                .AddField("Creator", $"[URL]({entry.CreatorURL})");

            if (entry.Type == "video") builder.WithThumbnail(entry.PreviewURL).Description += $"\n**[Video URL]({entry.URL})**";
            else builder.WithImageUrl(entry.URL);

            if (entries.Length >= 100) builder.WithFooter("Found 100 or more results for this query.");
            else builder.WithFooter($"Found ${entries.Length} results.");


            await context.RespondAsync(embed: builder.Build());
        }
    }
}
