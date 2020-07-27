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

using DSharpPlus.EventArgs;
using System.Threading.Tasks;

namespace Bot.Managers
{
    public class Starboard
    {
        public static Task ReactionAddHandler(MessageReactionAddEventArgs e)
        {
            return Task.CompletedTask;
        }

        public static Task ReactionRemoveHandler(MessageReactionRemoveEventArgs e)
        {
            return Task.CompletedTask;
        }

        public static Task ReactionRemoveEmojiHandler(MessageReactionRemoveEmojiEventArgs e)
        {
            return Task.CompletedTask;
        }

        public static Task ReactionRemoveAllHandler(MessageReactionsClearEventArgs e)
        {
            return Task.CompletedTask;
        }
    }
}
