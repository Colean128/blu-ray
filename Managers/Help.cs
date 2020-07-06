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

using Bot.Types;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.CommandsNext.Entities;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bot.Managers
{
    public class Help : BaseHelpFormatter
    {
        private DiscordEmbedBuilder builder;

        private CommandModule castModule(BaseCommandModule mod) => mod as CommandModule;

        public Help(CommandContext context) : base(context) => builder = new DiscordEmbedBuilder()
            .WithColor(new DiscordColor("4287f5"))
            .WithTitle("Help")
            .WithFooter("", context.Client.CurrentUser.GetAvatarUrl(ImageFormat.Png));

        public override CommandHelpMessage Build() => new CommandHelpMessage(embed: builder.Build());

        public override BaseHelpFormatter WithCommand(Command command)
        {
            builder.Description = $"Command: **{command.Name}**\n\nDescription: **{command.Description}**";

            if (command.Aliases.Count > 0)
            {
                builder.Description += "\nAliases:";
                command.Aliases.ToList().ForEach(x => builder.Description += $" `{x}`");
            }

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

            builder.Footer.Text = $"The command \"{command.Name}\" is inside the module \"{castModule(command.Module.GetInstance(null)).Name}\".";
            return this;
        }

        public override BaseHelpFormatter WithSubcommands(IEnumerable<Command> subcommands)
        {
            List<Command> commands = new List<Command>();
            foreach (Command cmd in subcommands) if (cmd.Name != "help") commands.Add(cmd);

            Dictionary<string, string> commandList = new Dictionary<string, string>();
            foreach (Command command in commands.OrderBy(c => castModule(c.Module.GetInstance(null)).Name))
            {
                CommandModule module = castModule(command.Module.GetInstance(null));
                if (!commandList.ContainsKey(module.Name)) commandList.Add(module.Name, "");

                commandList[module.Name] += $"- **{command.Name}**: `{command.Description}`\n";
            }

            foreach (string mod in commandList.Keys) builder.AddField(mod, commandList[mod]);

            builder.Description = "**Commands list**";
            builder.Footer.Text = $"There are {subcommands.ToList().Count} commands across {commandList.Keys.Count} modules, making an average of {Math.Truncate(Math.Round((double)subcommands.ToList().Count) / (double)commandList.Keys.Count)} commands per module.";
            return this;
        }
    }
}
