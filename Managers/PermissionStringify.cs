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

using DSharpPlus;
using System.Collections.Generic;

namespace Bot.Managers
{
    public class PermissionStringify
    {
        public static List<string> ConvertToString(Permissions permissions)
        {
            List<string> permissionList = new List<string>();

            if (permissions.HasPermission(Permissions.KickMembers))         permissionList.Add("Kick members");
            if (permissions.HasPermission(Permissions.BanMembers))          permissionList.Add("Ban members");
            if (permissions.HasPermission(Permissions.Administrator))       permissionList.Add("Administrator");
            if (permissions.HasPermission(Permissions.ManageChannels))      permissionList.Add("Manage channels");
            if (permissions.HasPermission(Permissions.ManageGuild))         permissionList.Add("Manage server settings");
            if (permissions.HasPermission(Permissions.AddReactions))        permissionList.Add("Add reactions to messages");
            if (permissions.HasPermission(Permissions.ViewAuditLog))        permissionList.Add("View audit log");
            if (permissions.HasPermission(Permissions.Stream))              permissionList.Add("Stream video");
            if (permissions.HasPermission(Permissions.ManageMessages))      permissionList.Add("Manage messages");
            if (permissions.HasPermission(Permissions.EmbedLinks))          permissionList.Add("Embed URLs");
            if (permissions.HasPermission(Permissions.AttachFiles))         permissionList.Add("Attach files");
            if (permissions.HasPermission(Permissions.ReadMessageHistory))  permissionList.Add("Read message history");
            if (permissions.HasPermission(Permissions.UseExternalEmojis))   permissionList.Add("Use external emojis");
            if (permissions.HasPermission(Permissions.UseVoice))            permissionList.Add("Connect to voice channels");
            if (permissions.HasPermission(Permissions.Speak))               permissionList.Add("Speak in voice channels");
            if (permissions.HasPermission(Permissions.MuteMembers))         permissionList.Add("Mute members server-wide");
            if (permissions.HasPermission(Permissions.DeafenMembers))       permissionList.Add("Deafen members server-wide");
            if (permissions.HasPermission(Permissions.MoveMembers))         permissionList.Add("Move members in voice chats");
            if (permissions.HasPermission(Permissions.ChangeNickname))      permissionList.Add("Change nickname");
            if (permissions.HasPermission(Permissions.ManageNicknames))     permissionList.Add("Manage nicknames of other members");
            if (permissions.HasPermission(Permissions.ManageRoles))         permissionList.Add("Manage roles");
            if (permissions.HasPermission(Permissions.ManageWebhooks))      permissionList.Add("Manage webhooks");
            if (permissions.HasPermission(Permissions.ManageEmojis))        permissionList.Add("Manage emojis");

            permissionList.Sort();
            return permissionList;
        }
    }
}
