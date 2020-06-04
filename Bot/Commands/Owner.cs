// Blu-Ray Discord Bot
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

using System;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace Bot.Commands
{
    public class Owner : BaseCommandModule
    {
        [Command("eval"), Hidden, RequireOwner, Aliases("evaluate")]
        public async Task EvaluateAsync(CommandContext context, [RemainingText] string code)
        {
            DiscordMessage message = await context.RespondAsync(embed: new DiscordEmbedBuilder()
                .WithTitle("Evaluation")
                .WithColor(0x3ba3ff)
                .WithDescription("Evaluation in progress...")
                .Build());

            try
            {
                ScriptOptions options = ScriptOptions.Default
                .WithImports("System", "System.Collections.Generic", "System.Linq", "System.Text", "System.Threading.Tasks",
                "DSharpPlus", "DSharpPlus.CommandsNext", "DSharpPlus.Entities", "DSharpPlus.Interactivity", "DSharpPlus.Net",
                "Bot", "Bot.Commands", "Bot.Managers")
                .WithReferences(AppDomain.CurrentDomain.GetAssemblies().Where(xa => !xa.IsDynamic && !string.IsNullOrWhiteSpace(xa.Location)));

                Script script = CSharpScript.Create(code, options, typeof(EvaluationContent));
                script.Compile();

                ScriptState result = await script.RunAsync(new EvaluationContent(context));

                if (result.Exception != null)
                {
                    await message.ModifyAsync(embed: new DiscordEmbedBuilder()
                        .WithTitle("Evaluation")
                        .WithColor(DiscordColor.Red)
                        .WithDescription($"Failure of type **\"{result.Exception.GetType().Name}\"**:\n```\n{result.Exception.Message}\n```")
                        .Build());
                    return;
                }

                if (result != null && result.ReturnValue != null && !string.IsNullOrWhiteSpace(result.ReturnValue.ToString()))
                {
                    await message.ModifyAsync(embed: new DiscordEmbedBuilder()
                        .WithTitle("Evaluation")
                        .WithColor(DiscordColor.Green)
                        .WithDescription($"Success.\n```\n{result.ReturnValue.ToString()}\n```")
                        .Build());
                    return;
                }

                await message.ModifyAsync(embed: new DiscordEmbedBuilder()
                    .WithTitle("Evaluation")
                    .WithColor(DiscordColor.Green)
                    .WithDescription($"Success. No content was returned.")
                    .Build());
            }
            catch (Exception ex)
            {
                await message.ModifyAsync(embed: new DiscordEmbedBuilder()
                    .WithTitle("Evaluation")
                    .WithColor(DiscordColor.Red)
                    .WithDescription($"An error of type **\"{ex.GetType().Name}\"**\n```\n{ex.Message}\n```")
                    .Build());
            }
        }

        public class EvaluationContent
        {
            public DiscordClient client { get; set; }
            public CommandsNextExtension commands { get; set; }
            public InteractivityExtension interactivity { get; set; }

            public DiscordChannel channel { get; set; }
            public DiscordGuild guild { get; set; }
            public DiscordMember member { get; set; }
            public DiscordMessage message { get; set; }
            public DiscordUser user { get; set; }

            public CommandContext context { get; set; }

            public EvaluationContent(CommandContext context)
            {
                client = context.Client;
                commands = context.Client.GetCommandsNext();
                interactivity = context.Client.GetInteractivity();

                channel = context.Channel;
                guild = context.Guild;
                member = context.Member;
                message = context.Message;
                user = context.User;

                this.context = context;
            }
        }
    }
}
