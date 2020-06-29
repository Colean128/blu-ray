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

            await context.Guild.BanMemberAsync(member, 0, $"{context.User.Username}#{context.User.Discriminator}{(reason != null ? $": {reason}" : "")}");
            await context.RespondAsync($"Banned user `{member.Username}#{member.Discriminator}` successfully.");
        }
    }
}
