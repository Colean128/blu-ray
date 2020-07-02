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

using Bot.Managers;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Bot.Commands
{
    public class Settings : BaseCommandModule
    {
        [Command("starboard"), Description("Sets the starboard for this server."), RequireGuild, RequirePermissions(Permissions.ManageChannels)]
        public async Task StarboardAsync(CommandContext context, [Description("Starboard channel to use; accepts either its ID or its tag.")] DiscordChannel channel = null, [Description("Emoji to use. Star emoji by default.")] DiscordEmoji emoji = null, [Description("Minimal amount of emojis required to make it onto the starboard, defaults to 2.")] int amount = 2)
        {
            if (channel == null)
            {
                await context.RespondAsync("Please provide a channel.");
                return;
            }

            if (emoji == null) emoji = DiscordEmoji.FromUnicode("⭐");
            if (emoji.Id != 0) try { DiscordEmoji.FromGuildEmote(context.Client, emoji.Id); }
                catch (KeyNotFoundException)
                {
                    await context.RespondAsync("The emoji you supplied couldn't be found.\nNote that Blu-Ray can only access emojis from other guilds if it's also a member there.");
                    return;
                }

            SqliteTransaction transaction   = Database.CreateTransaction();
            SqliteCommand command           = Database.CreateCommand();

            command.CommandText = "insert or replace into starboardChannels (guildId, channelId, emoji) values ($gid, $chn, $emj)";

            command.Parameters.Add(new SqliteParameter
            {
                ParameterName   = "$gid",
                Value           = context.Guild.Id
            });

            command.Parameters.Add(new SqliteParameter
            {
                ParameterName   = "$chn",
                Value           = channel.Id
            });

            command.Parameters.Add(new SqliteParameter
            {
                ParameterName   = "$emj",
                Value           = emoji.Id != 0 ? $"{emoji.Name}:{emoji.Id}" : emoji.Name
            });

            command.ExecuteNonQuery();
            transaction.Commit();

            await context.RespondAsync("Your new starboard has been set.", embed: new DiscordEmbedBuilder()
                .WithDescription($"Channel: **{channel.Mention}**\nEmoji: {(emoji.Id != 0 ? $"<:{emoji.Name}:{emoji.Id}>" : emoji.Name)}\nAmount of reactions necessary: **{amount}**")
                .WithTitle("Starboard properties")
                .Build());
        }
    }
}
