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
            .WithDescription("Hello!\nI'm **Blu-Ray**, a general purpose Discord bot.\nI feature variously different commands, with some new ones probably still in work.\nI hope I meet your bot-ly desires.\n\n")
            .AddField("Useful Links:", $"- [GitHub](https://github.com/Zayne64/blu-ray)\n- [Support Server](https://discord.gg/g2SWnrg)\n- [Minimal Invite](https://discord.com/api/oauth2/authorize?client_id={context.Client.CurrentApplication.Id}&permissions=0&scope=bot)\n- [Full Invite](https://discord.com/api/oauth2/authorize?client_id={context.Client.CurrentApplication.Id}&permissions={(int)(Permissions.AccessChannels | Permissions.AddReactions | Permissions.AttachFiles | Permissions.BanMembers | Permissions.ChangeNickname | Permissions.EmbedLinks | Permissions.KickMembers | Permissions.ManageEmojis | Permissions.ManageGuild | Permissions.ManageMessages | Permissions.ReadMessageHistory | Permissions.SendMessages | Permissions.Speak | Permissions.UseExternalEmojis | Permissions.UseVoice)}&scope=bot)")
            .Build());

        [Command("game"), Description("Shows what you're currently playing, or what someone else is playing."), Aliases("rpc", "status"), RequireGuild]
        public async Task GameAsync(CommandContext context, [RemainingText, Description("A member to check for. Can be left empty.")] DiscordMember member = null)
        {
            if (member == null) member = context.Member;

            if (member.Presence.Activity.Name == null)
            {
                await context.RespondAsync("You're not playing any game at the moment.");
                return;
            }
            else if (member.Presence.Activity.RichPresence != null && member.Presence.Activity.ActivityType == ActivityType.Custom)
            {
                await context.RespondAsync($"**{member.Username}#{member.Discriminator}** set a custom status: `{member.Presence.Activity.RichPresence.State}`");
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
                message += $" for ";

                TimeSpan span = (TimeSpan)(DateTime.Now - member.Presence.Activity.RichPresence.StartTimestamp);
                if (span.Minutes == 0) message += $"__{span.Seconds} second{(span.Seconds == 1 ? "" : "s")}__";
                else
                {
                    message += $"__{span.Minutes} minute{(span.Minutes == 1 ? "" : "s")}__";
                    if (span.Seconds != 0) message += $" and __{span.Seconds} second{(span.Seconds == 1 ? "" : "s")}__";
                }
            }

            await context.RespondAsync(message + ".", embed: builder.Build());
        }

        [Command("ping"), Description("Shows the ping of the bot."), Aliases("pong")]
        public async Task PingAsync(CommandContext context) => await context.RespondAsync($"{DiscordEmoji.FromName(context.Client, ":ping_pong:")} Pong! Ping: **{context.Client.Ping}ms**.");

        [Command("quote"), Description("Quote another user's message."), RequireGuild]
        public async Task QuoteAsync(CommandContext context, [Description("ID of the message to quote.")] ulong id = 0)
        {
            if (id == 0)
            {
                await context.RespondAsync("Please provide a message ID.");
                return;
            }

            await QuoteAsync(context, id, context.Channel);
        }

        [Command("quote")]
        public async Task QuoteAsync(CommandContext context, [Description("URL towards the message to quote.")] string url = null)
        {
            if (url == null)
            {
                await context.RespondAsync("Please provide a URL.");
                return;
            }

            ulong serId = 0, chnId = 0, msgId = 0;
            try
            {
                serId = ulong.Parse(url.Substring(32, 50));
                chnId = ulong.Parse(url.Substring(51, 69));
                msgId = ulong.Parse(url.Substring(70, 88));
            }
            catch (Exception)
            {
                await context.RespondAsync("You provided an invalid URL.");
                return;
            }

            if (serId == 0 || chnId == 0 || msgId == 0)
            {
                await context.RespondAsync("You provided an invalid URL.");
                return;
            }
            else if (serId != context.Guild.Id)
            {
                await context.RespondAsync("The referenced message is not from this server.");
                return;
            }

            await QuoteAsync(context, msgId, context.Guild.GetChannel(chnId));
        }

        [Command("quote")]
        public async Task QuoteAsync(CommandContext context, [Description("ID of the message to quote.")] ulong messageId = 0, [Description("ID or tag of the channel that contains the message.")] DiscordChannel channel = null)
        {
            if (messageId == 0)
            {
                await context.RespondAsync("Please provide a message ID.");
                return;
            }
            else if (channel == null)
            {
                await context.RespondAsync("Please provide a channel ID.");
                return;
            }
        }
    }
}
