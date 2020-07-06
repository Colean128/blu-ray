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
using DSharpPlus.Entities;

namespace Bot.Structures
{
    public class Emojis
    {
        public static string SuccessEmoji         { get; internal set; }
        public static string WarningEmoji         { get; internal set; }
        public static string ErrorEmoji           { get; internal set; }

        public static string OnlineEmoji          { get; internal set; }
        public static string IdleEmoji            { get; internal set; }
        public static string DoNotDisturbEmoji    { get; internal set; }
        public static string OfflineEmoji         { get; internal set; }

        public static void Initialize(ulong success, ulong warning, ulong error, ulong online, ulong idle, ulong dnd, ulong offline)
        {
            SuccessEmoji        = $"<:success:{success}>";
            WarningEmoji        = $"<:warning:{warning}>";
            ErrorEmoji          = $"<:error:{error}>";

            OnlineEmoji         = $"<:online:{online}>";
            IdleEmoji           = $"<:idle:{idle}>";
            DoNotDisturbEmoji   = $"<:dnd:{dnd}>";
            OfflineEmoji        = $"<:offline:{offline}>";
        }
    }
}
