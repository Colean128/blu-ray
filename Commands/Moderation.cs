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
using System.Threading.Tasks;

namespace Bot.Commands
{
    public class Moderation : CommandModule
    {
        public Moderation() => Name = "Moderation";

        [Command("ban"), Description("Bans a member."), RequirePermissions(Permissions.BanMembers), RequireGuild]
        public async Task BanAsync(CommandContext context, [Description("Member to ban.")] DiscordMember member = null, [RemainingText, Description("(Optional) Reason for ban.")] string reason = null)
        {
            if (member == null)
            {
                await context.RespondAsync($"{Emojis.WarningEmoji} Provide a member to ban.");
                return;
            }

            await member.BanAsync(0, $"{context.User.Username}#{context.User.Discriminator}{(reason != null ? $": {reason}" : "")}");
            await context.RespondAsync($"{Emojis.SuccessEmoji} Banned user `{member.Username}#{member.Discriminator}` successfully.");
        }

        [Command("ban")]
        public async Task BanAsync(CommandContext context, [Description("ID of the member to ban.")] ulong id = 0, [RemainingText, Description("(Optional) Reason for ban.")] string reason = null)
        {
            if (id == 0)
            {
                await context.RespondAsync($"{Emojis.WarningEmoji} Provide a ID to ban.");
                return;
            }

            DiscordUser user;
            try { user = await context.Client.GetUserAsync(id); }
            catch (Exception ex)
            {
                context.Client.DebugLogger.LogMessage(LogLevel.Error, "Commands - Ban", $"Failed to find the user for ID \"{id}\".", DateTime.Now, ex);
                await context.RespondAsync($"{Emojis.ErrorEmoji} Invalid ID.");
                return;
            }

            if (user == null)
            {
                await context.RespondAsync($"{Emojis.ErrorEmoji} Invalid ID.");
                return;
            }

            await context.Guild.BanMemberAsync(user.Id, 0, $"{context.User.Username}#{context.User.Discriminator}{(reason != null ? $": {reason}" : "")}");
            await context.RespondAsync($"{Emojis.SuccessEmoji} Banned user `{user.Username}#{user.Discriminator}` successfully.");
        }

        [Command("softban"), Description("Bans and immediately unbans the given member. This clears their messages, but behaves like a kick."), RequirePermissions(Permissions.BanMembers), RequireGuild]
        public async Task SoftbanAsync(CommandContext context, [Description("Member to softban.")] DiscordMember member = null, [RemainingText, Description("(Optional) Reason for unban.")] string reason = null)
        {
            if (member == null)
            {
                await context.RespondAsync($"{Emojis.WarningEmoji} Provide a member to softban.");
                return;
            }

            await context.Guild.BanMemberAsync(member, 7, $"{context.User.Username}#{context.User.Discriminator}{(reason != null ? $": {reason}" : "")} (softban 1/2)");
            await context.Guild.UnbanMemberAsync(member, $"{context.User.Username}#{context.User.Discriminator}{(reason != null ? $": {reason}" : "")} (softban 2/2)");
            await context.RespondAsync($"{Emojis.SuccessEmoji} Softbanned user **{member.Username}#{member.Discriminator}** successfully.");
        }

        [Command("unban"), Description("Unbans a user via their ID."), RequirePermissions(Permissions.BanMembers), RequireGuild]
        public async Task UnbanAsync(CommandContext context, [Description("User ID to unban.")] ulong id = 0, [RemainingText, Description("(Optional) Reason for unban.")] string reason = null)
        {
            if (id == 0)
            {
                await context.RespondAsync($"{Emojis.WarningEmoji} Provide a user ID to unban.");
                return;
            }

            DiscordUser user;
            try { user = await context.Client.GetUserAsync(id); }
            catch (NotFoundException)
            {
                await context.RespondAsync($"{Emojis.ErrorEmoji} Invalid user ID.");
                return;
            }

            if (user == null)
            {
                await context.RespondAsync($"{Emojis.ErrorEmoji} Invalid user ID.");
                return;
            }

            await context.Guild.UnbanMemberAsync(user, $"{context.User.Username}#{context.User.Discriminator}{(reason != null ? $": {reason}" : "")}");
            await context.RespondAsync($"{Emojis.SuccessEmoji} Unbanned **{user.Username}#{user.Discriminator}** successfully.");
        }

        [Command("kick"), Description("Kicks a member."), RequirePermissions(Permissions.KickMembers), RequireGuild]
        public async Task KickAsync(CommandContext context, [Description("Member to kick.")] DiscordMember member = null, [RemainingText, Description("(Optional) Reason for kick.")] string reason = null)
        {
            if (member == null)
            {
                await context.RespondAsync($"{Emojis.WarningEmoji} Provide a member to kick.");
                return;
            }

            await member.RemoveAsync($"{context.User.Username}#{context.User.Discriminator}{(reason != null ? $": {reason}" : "")}");
            await context.RespondAsync($"{Emojis.SuccessEmoji} Kicked user `{member.Username}#{member.Discriminator}` successfully.");
        }

        [Command("clean"), Description("Deletes a given amount of messages."), Aliases("clear"), RequirePermissions(Permissions.ManageMessages), RequireGuild]
        public async Task CleanAsync(CommandContext context, [Description("Amount of messages to clean. All messages within that amount before the triggering command will be deleted.")] int amount = 0)
        {
            if (amount == 0)
            {
                await context.RespondAsync($"{Emojis.WarningEmoji} Provide an amount of messages to clean.");
                return;
            }
            if (amount > 100)
            {
                await context.RespondAsync($"{Emojis.ErrorEmoji} The amount must be in between 1 and 100.");
                return;
            }

            await context.Channel.DeleteMessagesAsync(await context.Channel.GetMessagesBeforeAsync(context.Message.Id, amount), $"{context.User.Username}#{context.User.Discriminator}; cleared {amount} messages");
            await context.RespondAsync($"{Emojis.SuccessEmoji} Cleaned the last **{amount}** messages.");
        }
    }
}
