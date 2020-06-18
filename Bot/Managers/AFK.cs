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
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bot.Managers
{
    public class AFK
    {
        private static Dictionary<ulong, string> entries = new Dictionary<ulong, string>();

        public static void AddMember(ulong id, string reason) => entries.Add(id, reason);

        public static async Task AFKMessageHandler(MessageCreateEventArgs e)
        {
            if (e.Guild == null) return;
            
            if (entries.ContainsKey(e.Author.Id))
            {
                DiscordMember member = await e.Guild.GetMemberAsync(e.Author.Id);
                await member.SendMessageAsync("Welcome back, you're no longer AFK.");
                entries.Remove(e.Author.Id);
                return;
            }

            DiscordUser user = null;
            foreach (string part in e.Message.Content.Split(' '))
            {
                if (!part.Contains("<@!") && !part.Contains(">")) continue;
                
                try { user = await e.Client.GetUserAsync(Convert.ToUInt64(part.Substring(3, 18))); }
                catch (Exception) { continue; }

                if (user != null) break;
            }

            if (user == null || !entries.ContainsKey(user.Id)) return;

            await e.Channel.SendMessageAsync($"**{user.Username}#{user.Discriminator}** is AFK{(entries[user.Id] != null ? $": `{entries[user.Id]}`": "")}.");
        }
    }
}
