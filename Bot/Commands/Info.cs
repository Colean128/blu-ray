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

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Threading.Tasks;

namespace Bot.Commands
{
    public class Info : BaseCommandModule
    {
        [Command("about"), Description("Shows information about the bot.")]
        public async Task AboutAsync(CommandContext context) => await context.RespondAsync(embed: new DiscordEmbedBuilder()
            .WithAuthor($"{context.Client.CurrentUser.Username}#{context.Client.CurrentUser.Discriminator}", "https://github.com/Zayne64/blu-ray", context.Client.CurrentUser.GetAvatarUrl(ImageFormat.Png))
            .WithColor(DiscordColor.Gray)
            .WithDescription("Hello!\nI'm **Blu-Ray**, a general purpose Discord bot.\nI feature variously different commands, with some new ones probably still in work.\nI hope I meet your bot-ly desires.\n\nUseful links:\n")
            .AddField("GitHub", "**https://github.com/Zayne64/blu-ray")
            .Build());
            
        [Command("ping"), Description("Shows the ping of the bot."), Aliases("pong")]
        public async Task PingAsync(CommandContext context) => await context.RespondAsync($"{DiscordEmoji.FromName(context.Client, ":ping_pong:")} Pong! Ping: **{context.Client.Ping}ms**.");

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
                TimeSpan span = (TimeSpan)(DateTime.Now.ToUniversalTime() - member.Presence.Activity.RichPresence.StartTimestamp);
                message += $" for ";

                if (span.Minutes == 0) message += $"__{span.Seconds} second{(span.Seconds == 1 ? "" : "s")}__";
                else
                {
                    message += $"__{span.Minutes} minute{(span.Minutes == 1 ? "" : "s")}__";
                    if (span.Seconds != 0) message += $" and __{span.Seconds} second{(span.Seconds == 1 ? "" : "s")}__";
                }
            }
            
            await context.RespondAsync(message + ".", embed: builder.Build());
        }
    }
}
