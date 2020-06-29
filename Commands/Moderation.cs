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
using Bot.Structures;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Threading.Tasks;

namespace Bot.Commands
{
    class Moderation : BaseCommandModule
    {
        [Command("ban"), Description("Ban a user."), RequirePermissions(Permissions.BanMembers), RequireGuild]
        public async Task BRBanAsync(CommandContext context, [Description("Ban a user."), RemainingText] DiscordMember member = null)
        {
            if (member == null)
            {
                await context.RespondAsync("You can't ban nobody!");
                return;
            }

            await context.Guild.BanMemberAsync(member, 0, $"Banned with Blu-Ray by {context.User.Username}.");
        }
    }
}
