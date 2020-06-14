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
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

namespace Bot.Commands
{
    public class Info : BaseCommandModule
    {
        [Command("game"), Description("Shows what you're currently playing, or what someone else is playing."), Aliases("rpc", "status"), RequireGuild]
        public async Task GameAsync(CommandContext context, [RemainingText, Description("A member to check for. Can be left empty.")] DiscordMember member = null)
        {
            if (member == null) member = context.Member;

            if (member.Presence.Activity.Name == null)
            {
                await context.RespondAsync("You're not playing any game at the moment.");
                return;
            }

            string message = $"**{member.Username}#{member.Discriminator}** has been playing **{member.Presence.Activity.Name}**";
            if (member.Presence.Activity.RichPresence == null)
            {
                await context.RespondAsync(message + ".");
                return;
            }

            DiscordEmbedBuilder builder = new DiscordEmbedBuilder()
                .WithTitle(member.Presence.Activity.Name)
                .WithDescription($"{(member.Presence.Activity.RichPresence.State != null ? member.Presence.Activity.RichPresence.State + "\n" : "")}{(member.Presence.Activity.RichPresence.State != null ? member.Presence.Activity.RichPresence.Details : "")}");
            
            if (member.Presence.Activity.RichPresence.LargeImage != null) builder.WithThumbnail(member.Presence.Activity.RichPresence.LargeImage.Url);

            if (member.Presence.Activity.RichPresence.StartTimestamp != null)
            {
                DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                TimeSpan span = (TimeSpan)(member.Presence.Activity.RichPresence.StartTimestamp - origin.ToUniversalTime() );
                message += $" for ";
                if (span.Minutes == 0) message += $"{span.Seconds} seconds.";
                else message += $"{span.Minutes} minutes.";
            }
            else message += ".";
            
            await context.RespondAsync(message, embed: builder.Build());
        }
    }
}