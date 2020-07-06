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
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.CommandsNext.Entities;
using DSharpPlus.Entities;
using System.Collections.Generic;
using System.Linq;

namespace Bot.Managers
{
    public class Help : BaseHelpFormatter
    {
        private DiscordEmbedBuilder builder;
        private string iconUrl;

        public Help(CommandContext context) : base(context)
        {
            builder = new DiscordEmbedBuilder().WithColor(new DiscordColor("4287f5")).WithTitle("Help");
            iconUrl = context.Client.CurrentUser.GetAvatarUrl(ImageFormat.Png);
        }

        public override CommandHelpMessage Build() => new CommandHelpMessage(embed: builder.Build());

        public override BaseHelpFormatter WithCommand(Command command)
        {
            builder.Description += $"- Command **{command.Name}** -\nDescription: **{command.Description}**";
            if (command.Overloads.Count > 1 || command.Overloads.Count == 1 && command.Overloads[0].Arguments.Count > 0)
            {
                builder.Description += "\nArguments:";
                foreach (CommandOverload overload in command.Overloads)
                {
                    builder.Description += $"\n- ";
                    overload.Arguments.ToList().ForEach(argument => builder.Description += $" `{argument.Name}`");
                    overload.Arguments.ToList().ForEach(argument => builder.Description += $"\n`{argument.Name}`: **{argument.Description}**");
                    builder.Description += "\n";
                }
            }

            return this;
        }

        public override BaseHelpFormatter WithSubcommands(IEnumerable<Command> subcommands)
        {
            builder.Description += "- Commands list -\n";
            subcommands.ToList().ForEach(x => builder.Description += $"- **{x.Name}**: `{x.Description}`\n");

            builder.Footer = new DiscordEmbedBuilder.EmbedFooter
            {
                Text    = $"There are {subcommands.ToList().Count} commands.",
                IconUrl = iconUrl
            };

            return this;
        }
    }
}
