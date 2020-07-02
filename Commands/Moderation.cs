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
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bot.Commands
{
    public class Moderation : BaseCommandModule
    {
        [Command("ban"), Description("Bans a member."), RequirePermissions(Permissions.BanMembers), RequireGuild]
        public async Task BanAsync(CommandContext context, [Description("Member to ban.")] DiscordMember member = null, [RemainingText, Description("(Optional) Reason for ban.")] string reason = null)
        {
            if (member == null)
            {
                await context.RespondAsync("Provide a member to ban.");
                return;
            }

            await member.BanAsync(0, $"{context.User.Username}#{context.User.Discriminator}{(reason != null ? $": {reason}" : "")}");
            await context.RespondAsync($"Banned user `{member.Username}#{member.Discriminator}` successfully.");
        }

        [Command("ban")]
        public async Task BanAsync(CommandContext context, [Description("ID of the member to ban.")] ulong id = 0, [RemainingText, Description("(Optional) Reason for ban.")] string reason = null)
        {
            if (id == 0)
            {
                await context.RespondAsync("Provide a ID to ban.");
                return;
            }

            DiscordUser user;
            try { user = await context.Client.GetUserAsync(id); }
            catch (Exception ex)
            {
                context.Client.DebugLogger.LogMessage(LogLevel.Error, "Commands - Ban", $"Failed to find the user for ID \"{id}\".", DateTime.Now, ex);
                await context.RespondAsync("Invalid ID.");
                return;
            }

            if (user == null)
            {
                await context.RespondAsync("Invalid ID.");
                return;
            }

            await context.Guild.BanMemberAsync(user.Id, 0, $"{context.User.Username}#{context.User.Discriminator}{(reason != null ? $": {reason}" : "")}");
            await context.RespondAsync($"Banned user `{user.Username}#{user.Discriminator}` successfully.");
        }

        [Command("softban"), Description("Bans and immediately unbans a member to clear messages."), RequirePermissions(Permissions.BanMembers), RequireGuild]
        public async Task SoftbanAsync(CommandContext context, [Description("Member to softban.")] DiscordMember member = null)
        {
            if (member == null)
            {
                await context.RespondAsync("Provide a member to softban.");
                return;
            }

            await context.Guild.BanMemberAsync(member, 7, $"{context.User.Username}#{context.User.Discriminator}: Softban with Blu-Ray.");
            await context.Guild.UnbanMemberAsync(member, $"{context.User.Username}#{context.User.Discriminator}: Softban with Blu-Ray.");
            await context.RespondAsync($"Softbanned user `{member.Username}#{member.Discriminator}` successfully.");
        }

        [Command("unban"), Description("Unbans a user via their ID."), RequirePermissions(Permissions.BanMembers), RequireGuild]
        public async Task UnbanAsync(CommandContext context, [Description("User ID to unban.")] ulong id = 0, [RemainingText, Description("(Optional) Reason for unban.")] string reason = null)
        {
            if (id == 0)
            {
                await context.RespondAsync("Please provide a ID to unban.");
                return;
            }

            DiscordUser user;
            try { user = await context.Client.GetUserAsync(id); }
            catch (Exception ex)
            {
                context.Client.DebugLogger.LogMessage(LogLevel.Error, "Commands - Unban", $"Failed to find the user for ID \"{id}\".", DateTime.Now, ex);
                await context.RespondAsync("Invalid ID.");
                return;
            }

            if (user == null)
            {
                await context.RespondAsync("Invalid ID.");
                return;
            }

            await context.Guild.UnbanMemberAsync(user, $"{context.User.Username}#{context.User.Discriminator}{(reason != null ? $": {reason}" : "")}");
        }

        [Command("kick"), Description("Kicks a member."), RequirePermissions(Permissions.KickMembers), RequireGuild]
        public async Task KickAsync(CommandContext context, [Description("Member to kick.")] DiscordMember member = null, [RemainingText, Description("(Optional) Reason for kick.")] string reason = null)
        {
            if (member == null)
            {
                await context.RespondAsync("Provide a member to kick.");
                return;
            }

            await member.RemoveAsync($"{context.User.Username}#{context.User.Discriminator}{(reason != null ? $": {reason}" : "")}");
            await context.RespondAsync($"Kicked user `{member.Username}#{member.Discriminator}` successfully.");
        }

        [Command("clean"), Description("Deletes a given amount of messages."), RequirePermissions(Permissions.ManageMessages), RequireGuild, Aliases("clear", "empty")]
        public async Task CleanAsync(CommandContext context, [Description("Amount of messages to clean. All messages within that amount before the triggering command will be deleted.")] int amount = 0)
        {
            if (amount == 0)
            {
                await context.RespondAsync("Provide an amount of messages to clean.");
                return;
            }

            IEnumerable<DiscordMessage> messages = await context.Channel.GetMessagesBeforeAsync(context.Message.Id, amount);
            await context.Channel.DeleteMessagesAsync(messages, $"{context.User.Username}#{context.User.Discriminator}; cleared {amount} messages");
            await context.RespondAsync($"Cleaned the last {amount} messages.");
        }
    }
}
