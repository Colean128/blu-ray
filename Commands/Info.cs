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

using Bot.Structures;
using Bot.Types;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Exceptions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bot.Commands
{
    public class Info : CommandModule
    {
        public Info() => Name = "Info";

        [Command("about"), Description("Shows information about the bot.")]
        public async Task AboutAsync(CommandContext context) => await context.RespondAsync(embed: new DiscordEmbedBuilder()
            .WithAuthor($"{context.Client.CurrentUser.Username}#{context.Client.CurrentUser.Discriminator}", "https://github.com/Zayne64/blu-ray", context.Client.CurrentUser.GetAvatarUrl(ImageFormat.Png))
            .WithColor(DiscordColor.Gray)
            .WithDescription("Hello!\nI'm **Blu-Ray**, a general purpose Discord bot.\nI feature variously different commands, with some new ones probably still in work.\nI hope I meet your bot-ly desires.\n\n")
            .AddField("Useful Links:", $"- [GitHub](https://github.com/Zayne64/blu-ray)\n- [Support Server](https://discord.gg/g2SWnrg)\n- [Minimal Invite](https://discord.com/api/oauth2/authorize?client_id={context.Client.CurrentApplication.Id}&permissions=0&scope=bot)\n- [Full Invite](https://discord.com/api/oauth2/authorize?client_id={context.Client.CurrentApplication.Id}&permissions={(int)(Permissions.AccessChannels | Permissions.AddReactions | Permissions.AttachFiles | Permissions.BanMembers | Permissions.ChangeNickname | Permissions.EmbedLinks | Permissions.KickMembers | Permissions.ManageEmojis | Permissions.ManageGuild | Permissions.ManageMessages | Permissions.ReadMessageHistory | Permissions.SendMessages | Permissions.Speak | Permissions.UseExternalEmojis | Permissions.UseVoice)}&scope=bot)")
            .Build());

        [Command("avatar"), Description("Replies with the user's avatar."), Aliases("pfp"), RequireGuild]
        public async Task AvatarAsync(CommandContext context, [RemainingText, Description("The member whose avatar you want.")] DiscordMember member = null)
        {
            if (member == null) member = context.Member;

            string imageUrl = member.GetAvatarUrl(member.AvatarHash.StartsWith("a_") ? ImageFormat.Gif : ImageFormat.Png);
            await context.RespondAsync(embed: new DiscordEmbedBuilder()
                .WithAuthor($"{member.Username}#{member.Discriminator}", imageUrl)
                .WithColor(ColorDetection.GetMostUsedColor(imageUrl))
                .WithImageUrl(imageUrl)
                .Build());
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

            ulong[] ids = new ulong[3];
            try { for (int i = 0; i != 3; i++) ids[i] = ulong.Parse(url.Substring(url.LastIndexOf("/") - (19 * i) + 1, 18)); }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.GetType().FullName}: \"{ex.Message}\"");
                return;
            }

            if (ids[0] == 0 || ids[1] == 0 || ids[2] == 0)
            {
                await context.RespondAsync("You provided an invalid URL.");
                return;
            }
            else if (ids[2] != context.Guild.Id)
            {
                await context.RespondAsync("The referenced message is not from this server.");
                return;
            }

            await QuoteAsync(context, ids[0], context.Guild.GetChannel(ids[1]));
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

            DiscordMessage quote = null;
            try { quote = await channel.GetMessageAsync(messageId); }
            catch (NotFoundException)
            {
                await context.RespondAsync("Failed to find the message.");
                return;
            }
            catch (UnauthorizedException)
            {
                await context.RespondAsync("Cannot access the message.");
                return;
            }

            await context.RespondAsync(embed: new DiscordEmbedBuilder()
                .WithAuthor($"{quote.Author.Username}#{quote.Author.Discriminator}", quote.JumpLink.ToString(), quote.Author.AvatarUrl)
                .WithColor(ColorDetection.GetMostUsedColor(quote.Author.GetAvatarUrl(ImageFormat.Png)))
                .WithDescription(quote.Content)
                .WithFooter("Message created:")
                .WithTimestamp(quote.CreationTimestamp)
                .Build());
        }

        [Command("server"), Description("Shows information about the current server."), Aliases("guild"), RequireGuild]
        public async Task ServerAsync(CommandContext context)
        {
            IReadOnlyCollection<DiscordMember> members = await context.Guild.GetAllMembersAsync();

            DiscordEmbedBuilder builder = new DiscordEmbedBuilder()
                .WithDescription($"- ID: **{context.Guild.Id}**\n- Owner: **{context.Guild.Owner.Username}#{context.Guild.Owner.Discriminator}** (ID: **{context.Guild.Owner.Id}**)\n- Voice region: **{context.Guild.VoiceRegion.Name}**")
                .WithTitle(context.Guild.Name);

            int online = 0, idle = 0, dnd = 0, offline = 0;
            foreach (DiscordMember member in members)
            {
                if (member.Presence == null)
                {
                    offline++;
                    continue;
                }

                switch (member.Presence.Status)
                {
                    case UserStatus.Online:
                        online++;
                        break;

                    case UserStatus.Idle:
                        idle++;
                        break;

                    case UserStatus.DoNotDisturb:
                        dnd++;
                        break;

                    default:
                        offline++;
                        break;
                }
            }

            builder.Description += $"\n- Members: **{members.Count}** (**{online}** {Emojis.OnlineEmoji} **{idle}** {Emojis.IdleEmoji} **{dnd}** {Emojis.DoNotDisturbEmoji} **{offline}** {Emojis.OfflineEmoji})";

            builder.Description += "\n - Premium Tier: **";
            switch (context.Guild.PremiumTier)
            {
                case PremiumTier.None:
                    builder.Description += "None";
                    break;

                case PremiumTier.Tier_1:
                    builder.Description += "Tier 1";
                    break;

                case PremiumTier.Tier_2:
                    builder.Description += "Tier 2";
                    break;

                case PremiumTier.Tier_3:
                    builder.Description += "Tier 3";
                    break;

                default:
                    builder.Description += "Unknown";
                    break;
            }

            builder.Description += $"** (Boosts: **{context.Guild.PremiumSubscriptionCount}**)";

            if (context.Guild.IconUrl != null)
            {
                builder.WithColor(ColorDetection.GetMostUsedColor(context.Guild.IconUrl.Replace(".jpg", ".png")));
                if (context.Guild.IconHash.StartsWith("a_")) builder.WithThumbnail(context.Guild.IconUrl.Replace(".jpg", ".gif"));
                else builder.WithThumbnail(context.Guild.IconUrl.Replace(".jpg", ".png"));
            }

            if (context.Guild.Features.Count > 0)
            {
                builder.Description += "\n- Features: ";
                Stringify.GuildFeatures(context.Guild.Features).ForEach(x => builder.Description += $"**{x}**, ");
                builder.Description = builder.Description.Substring(0, builder.Description.LastIndexOf(", "));
            }

            await context.RespondAsync(embed: builder.Build());
        }

        [Command("status"), Description("Present your or someone else's status."), Aliases("rpc", "game"), RequireGuild]
        public async Task GameAsync(CommandContext context, [RemainingText, Description("The member to take the status from. Leave this empty if you're presenting your own status.")] DiscordMember member = null)
        {
            if (member == null) member = context.Member;

            if (member.Presence.Activity.Name == null)
            {
                await context.RespondAsync($"{(member != context.Member ? $"{member.Username}#{member.Discriminator} isn't" : "You're not")} doing anything at the moment.");
                return;
            }

            DiscordEmbedBuilder builder = new DiscordEmbedBuilder();
            if (member.Presence.Activities.Count == 1)
            {
                if (member.Presence.Activity.ActivityType == ActivityType.Custom)
                {
                    await context.RespondAsync($"**{member.Username}#{member.Discriminator}** set a custom status: {(member.Presence.Activity.CustomStatus.Emoji != null ? member.Presence.Activity.CustomStatus.Emoji.ToString() + " ": "")}{(member.Presence.Activities[0].CustomStatus.Name != null ? member.Presence.Activity.CustomStatus.Name : "")}");
                    return;
                }

                string type = "playing";
                switch (member.Presence.Activity.ActivityType)
                {
                    case ActivityType.ListeningTo:
                        type = "listening to";
                        break;

                    case ActivityType.Streaming:
                        type = "streaming";
                        break;

                    case ActivityType.Watching:
                        type = "watching";
                        break;
                }

                string message = $"**{member.Username}#{member.Discriminator}** has been {type} **{member.Presence.Activity.Name}**";
                if (member.Presence.Activity.RichPresence.Application == null)
                {
                    await context.RespondAsync(message + ".");
                    return;
                }

                builder.WithTitle(member.Presence.Activity.Name)
                    .WithDescription($"{(member.Presence.Activity.RichPresence.Details != null ? member.Presence.Activity.RichPresence.Details + "\n" : "")}{(member.Presence.Activity.RichPresence.State != null ? member.Presence.Activity.RichPresence.State : "")}");

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
                return;
            }

            foreach (DiscordActivity activity in member.Presence.Activities)
            {
                if (activity.ActivityType == ActivityType.Custom)
                {
                    builder.AddField("Custom Status", $"{(member.Presence.Activity.CustomStatus.Emoji != null ? member.Presence.Activity.CustomStatus.Emoji.ToString() + " " : "")}{(member.Presence.Activities[0].CustomStatus.Name != null ? member.Presence.Activity.CustomStatus.Name : "")}", builder.Fields.Count % 2 == 0);
                    continue;
                }

                string type = "Playing";
                switch (member.Presence.Activity.ActivityType)
                {
                    case ActivityType.ListeningTo:
                        type = "Listening to";
                        break;

                    case ActivityType.Streaming:
                        type = "Streaming";
                        break;

                    case ActivityType.Watching:
                        type = "Watching";
                        break;
                }

                string time = null;
                if (member.Presence.Activity.RichPresence.StartTimestamp != null)
                {
                    time += "For ";

                    TimeSpan span = (TimeSpan)(DateTime.Now - member.Presence.Activity.RichPresence.StartTimestamp);
                    if (span.Minutes == 0) time += $"__{span.Seconds} second{(span.Seconds == 1 ? "" : "s")}__";
                    else
                    {
                        time += $"__{span.Minutes} minute{(span.Minutes == 1 ? "" : "s")}__";
                        if (span.Seconds != 0) time += $" and __{span.Seconds} second{(span.Seconds == 1 ? "" : "s")}__";
                    }

                    time += ".";
                }

                builder.AddField($"{type} {activity.Name}", $"{(member.Presence.Activity.RichPresence.Details != null ? member.Presence.Activity.RichPresence.Details + "\n" : "")}{(member.Presence.Activity.RichPresence.State != null ? member.Presence.Activity.RichPresence.State + "\n" : "")}{(time != null ? time : "")}", builder.Fields.Count % 2 == 0);
            }

            await context.RespondAsync($"**{member.Username}#{member.Discriminator}** is doing various things at the moment.", embed: builder.Build());
        }
    }
}
