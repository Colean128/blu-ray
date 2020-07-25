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
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext.Exceptions;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bot.Managers
{
    public class Events
    {
        internal static ulong supportGuildId;
        
        public static Task OnClientReady(ReadyEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "Client", $"The client is now ready. Connected as {e.Client.CurrentUser.Username}#{e.Client.CurrentUser.Discriminator} (ID: {e.Client.CurrentUser.Id}).", DateTime.Now);
            return Task.CompletedTask;
        }

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

            int percentage = Convert.ToInt32(Math.Round((double)botAmount / (double)members.Count * 100.0f));
            if (e.Guild.Id != supportGuildId && members.Count > 5 && percentage >= 20)
            {
                e.Client.DebugLogger.LogMessage(LogLevel.Info, "Guilds", $"Joined guild \"{e.Guild.Name}\" (ID: {e.Guild.Id}). High bot density: {percentage}% ({members.Count} - {botAmount}).", DateTime.Now);

                if (currentMember == null)
                {
                    await e.Guild.LeaveAsync();
                    return;
                }

                IReadOnlyList<DiscordChannel> channels = await e.Guild.GetChannelsAsync();
                
                for (int i = 0; i != channels.Count; i++) if (channels[i].Type == ChannelType.Text && channels[i].PermissionsFor(currentMember).HasPermission(Permissions.SendMessages)) try
                {
                    await channels[i].SendMessageAsync(embed: new DiscordEmbedBuilder()
                        .WithAuthor($"{e.Client.CurrentUser.Username}#{e.Client.CurrentUser.Discriminator}", "https://github.com/Zayne64/Blu-Ray", currentMember.GetAvatarUrl(ImageFormat.Png))
                        .WithDescription($"Hello everyone!\nWe're sorry to remind you that Blu-Ray has to leave this guild.\n{percentage}% of all members in this server are bots, which is more than the allowed 20% bot density for servers, if they want to use the Blu-Ray Discord bot.\nDue to limited resources, we cannot, and, even if this wouldn't be a problem, we will not support the idea of bot farms.\n\nIf you still want to use Blu-Ray, remove other bots, please.\n\nIf you believe this judgment is invalid, try adding Blu-Ray again.\nIf Blu-Ray persists on leaving and you're still sure my judgment is false, you can leave an issue on the GitHub repository: https://github.com/Zayne64/blu-ray\n\nSincerely,\nthe team behind the Blu-Ray bot.")
                        .WithTitle("High bot density")
                        .Build());

                    break;
                }
                catch (Exception) { continue; }

                await e.Guild.LeaveAsync();
                return;
            }

            e.Client.DebugLogger.LogMessage(LogLevel.Info, "Guilds", $"Joined guild \"{e.Guild.Name}\" (ID: {e.Guild.Id}). Bot density: {percentage}% ({members.Count} - {botAmount}).", DateTime.Now);
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

            if (e.Context.Guild == null || (e.Context.Channel.PermissionsFor(e.Context.Member) & Permissions.SendMessages) != 0)
            {
                switch (e.Exception)
                {
                    case ArgumentException _:
                        await e.Context.RespondAsync($"{Emojis.ErrorEmoji} Invalid input.");
                        return;

                    case ChecksFailedException checks:
                        switch (checks.FailedChecks.FirstOrDefault())
                        {
                            case RequirePermissionsAttribute attr:
                                string perms = null;
                                Stringify.RolePermissions(attr.Permissions).ForEach((x) => perms += $"- **{x}**\n");

                                await e.Context.RespondAsync($"{Emojis.ErrorEmoji} The command \"`{e.Command.Name}`\" requires elevated permissions.", embed: new DiscordEmbedBuilder()
                                    .WithDescription(perms)
                                    .WithTitle("Required permissions")
                                    .Build());
                                break;

                            case RequireDirectMessageAttribute _:
                                await e.Context.RespondAsync($"{Emojis.ErrorEmoji} The command \"`{e.Command.Name}`\" is restricted to direct messages only.");
                                break;

                            case RequireGuildAttribute _:
                                await e.Context.RespondAsync($"{Emojis.ErrorEmoji} The command \"`{e.Command.Name}`\" is restricted to servers only.");
                                break;

                            case RequireNsfwAttribute _:
                                await e.Context.RespondAsync($"{Emojis.ErrorEmoji} The command \"`{e.Command.Name}`\" cannot be executed outside of a channel set as NSFW one.");
                                break;

                            case RequireRolesAttribute attr:
                                await e.Context.RespondAsync($"{Emojis.ErrorEmoji} The command \"`{e.Command.Name}`\" requires you to possess {(attr.RoleNames.Count == 1 ? "a " : "")}certain role{(attr.RoleNames.Count > 1 ? "s" : "")}: {attr.RoleNames.Aggregate((a, b) => a + ", " + b)}");
                                break;

                            default:
                                break;
                        }

                        return;

                    default:
                        break;
                }
            }

            e.Context.Client.DebugLogger.LogMessage(LogLevel.Error, "Commands", $"The command \"{e.Command.Name}\" executed by the user {e.Context.User.Username}#{e.Context.User.Discriminator} (ID: {e.Context.User.Id}) in channel \"{e.Context.Channel.Id}\" encountered an error.", DateTime.Now, e.Exception);
            if (e.Context.Guild == null || (e.Context.Channel.PermissionsFor(e.Context.Member) & Permissions.SendMessages) != 0) await e.Context.RespondAsync("An error occurred.");
        }
    }
}
