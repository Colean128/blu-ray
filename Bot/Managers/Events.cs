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
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bot.Managers
{
    public class Events
    {
        public static Task OnClientResumed(ReadyEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "Client", "Resumed session.", DateTime.Now);
            return Task.CompletedTask;
        }

        public static Task OnClientError(ClientErrorEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(LogLevel.Error, "Client", $"The client encountered an error.", DateTime.Now, e.Exception);
            return Task.CompletedTask;
        }

        public static Task OnClientSocketError(SocketErrorEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(LogLevel.Error, "Client", $"The client's connection encountered an error.", DateTime.Now, e.Exception);
            return Task.CompletedTask;
        }

        public static async Task OnGuildJoin(GuildCreateEventArgs e)
        {
            IReadOnlyCollection<DiscordMember> members = await e.Guild.GetAllMembersAsync();

            int botAmount = 0;
            DiscordMember currentMember = null;
            foreach (DiscordMember member in members)
            {
                if (member.IsCurrent) currentMember = member;
                if (member.IsBot) botAmount++;
            }

            if (currentMember == null) await e.Guild.LeaveAsync();

            int percentage = botAmount / members.Count * 100;
            if (members.Count > 5 && percentage >= 20)
            {
                e.Client.DebugLogger.LogMessage(LogLevel.Info, "Guilds", $"Joined guild \"{e.Guild.Name}\" (ID: {e.Guild.Id}). High bot density: {percentage}% ({members.Count} - {botAmount}).", DateTime.Now);

                IReadOnlyList<DiscordChannel> channels = await e.Guild.GetChannelsAsync();
                
                for (int i = 0; i != members.Count + 1; i++) if (channels[i].PermissionsFor(currentMember).HasPermission(Permissions.SendMessages))
                {
                    await channels[i].SendMessageAsync(embed: new DiscordEmbedBuilder().WithTitle("High bot density").WithDescription($"Hello everyone!\nI'm sorry to remind you that I'll have to leave this guild.\n{percentage}% of all members in this server are bots, which is more than the allowed amount  of bots that servers can have, if they want to use Blu-Ray.\nDue to limited resources, we cannot, and, even if this wouldn't be a problem, we will not support the idea of bot farms.\n\nIf you still want to use Blu-Ray, remove other bots, please.\n\nIf you believe this judgement is invalid, you can leave an issue on the GitHub repository: https://github.com/Zayne64/blu-ray\n\nSincerely,\nthe team behind the Blu-Ray bot.").Build());
                    break;
                }

                await e.Guild.LeaveAsync();
                return;
            }

            e.Client.DebugLogger.LogMessage(LogLevel.Info, "Guilds", $"Joined guild \"{e.Guild.Name}\" (ID: {e.Guild.Id}).", DateTime.Now);
        }

        public static Task OnGuildLeave(GuildDeleteEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "Guilds", $"Left guild \"{e.Guild.Name}\" (ID: {e.Guild.Id}).", DateTime.Now);
            return Task.CompletedTask;
        }

        public static Task OnCommandExecute(CommandExecutionEventArgs e)
        {
            e.Context.Client.DebugLogger.LogMessage(LogLevel.Info, "Commands", $"User {e.Context.User.Username}#{e.Context.User.Discriminator} (ID: {e.Context.User.Id}) executed command \"{e.Command.Name}\" in channel \"{e.Context.Channel.Id}\".", DateTime.Now);
            return Task.CompletedTask;
        }

        public static async Task OnCommandError(CommandErrorEventArgs e)
        {
            if (e.Command == null) return;

            e.Context.Client.DebugLogger.LogMessage(LogLevel.Error, "Commands", $"The command \"{e.Command.Name}\" executed by the user {e.Context.User.Username}#{e.Context.User.Discriminator} (ID: {e.Context.User.Id}) in channel \"{e.Context.Channel.Id}\" encountered an error.", DateTime.Now, e.Exception);
            
            await e.Context.RespondAsync("An error occurred.\nIf this persists, use the `about` command and leave an issue on the GitHub repository.\n");
        }
    }
}
