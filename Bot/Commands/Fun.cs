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

using Bot.Structures;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Threading.Tasks;

namespace Bot.Commands
{
    public class Fun : BaseCommandModule
    {
        private readonly string[] eightballResponses =
        {
            "It is certain",
            "Without a doubt",
            "Yes – definitely",
            "You may rely on it",
            "As I see it",
            "Most likely",
            "Outlook good",
            "Yes",
            "Signs point to yes",
            "It is decidedly so",
            "Reply hazy",
            "Ask again later",
            "Better not tell you now",
            "Cannot predict now",
            "Concentrate and ask again",
            "Don\"t count on it",
            "My reply is no",
            "My sources say no",
            "Outlook not so good",
            "Very doubtful"
        };

        private readonly string[] brunoVideos =
        {
            "https://www.youtube.com/watch?v=6LvlG2dTQKg",
            "https://www.youtube.com/watch?v=ILvd5buCEnU",
            "https://www.youtube.com/watch?v=nEDw_WKeQoc",
            "https://www.youtube.com/watch?v=0YrU9ASVw6w",
            "https://www.youtube.com/watch?v=GxMXWqSauZA",
            "https://www.youtube.com/watch?v=9rtD2omE2N0",
            "https://www.youtube.com/watch?v=-Tqn5NqXskM"
        };

        [Command("8ball"), Description("Ask the magic 8-Ball a question."), Aliases("eightball", "oracle")]
        public async Task EightballAsync(CommandContext context, [Description("The question to ask the 8-Ball about."), RemainingText] string question) => await context.RespondAsync(embed: new DiscordEmbedBuilder()
            .WithAuthor($"{context.User.Username}", iconUrl: context.User.AvatarUrl)
            .WithDescription($"You've asked the magic 8-Ball the following question:\n```\n{question}\n```\nMy Answer is: **{eightballResponses[new Random().Next(0, eightballResponses.Length - 1)]}**."));

        [Command("bruno"), Description("Shows you a random Bruno Powroznik video."), Aliases("powroznik")]
        public async Task BrunoAsync(CommandContext context) => await context.RespondAsync($"Here's a Bruno Powroznik video for you:\n\n{brunoVideos[new Random().Next(0, brunoVideos.Length - 1)]}");

        [Command("cat"), Description("Shows a cute picture of a cat."), Aliases("meow")]
        public async Task CatAsync(CommandContext context) => await context.RespondAsync(embed: new DiscordEmbedBuilder().WithTitle("Here's your cat picture!").WithImageUrl(await NekosLifeImage.GetAsync(NekosLifeImage.Endpoint.Meow)));

        [Command("say"), Description("Make the bot say something funny!")]
        public async Task SayAsync(CommandContext context, [Description("Make the bot say something funny!"), RemainingText] string text) => await context.RespondAsync($"{text}");

        [Command("dice"), Description("Rolls a dice of a range from 1 to 6."), Aliases("roll")]
        public async Task DiceAsync(CommandContext context) => await context.RespondAsync($"You rolled the dice. You got a **{new Random().Next(1, 6)}**.");

        [Command("dog"), Description("Shows a cute picture of a dog."), Aliases("woof")]
        public async Task DogAsync(CommandContext context) => await context.RespondAsync(embed: new DiscordEmbedBuilder().WithTitle("Here's your dog picture!").WithImageUrl(await NekosLifeImage.GetAsync(NekosLifeImage.Endpoint.Woof)));

        [Command("hug"), Description("Hugs another member."), Aliases("squeeze"), RequireGuild]
        public async Task HugAsync(CommandContext context, [RemainingText, Description("Member to hug.")] DiscordMember member = null)
        {
            if (member == null)
            {
                await context.RespondAsync("You need to name someone to hug.");
                return;
            }

            await context.RespondAsync(embed: new DiscordEmbedBuilder().WithTitle($"{context.User.Username} hugged {member.Username}.").WithImageUrl(await NekosLifeImage.GetAsync(NekosLifeImage.Endpoint.Hug)));
        }

        [Command("kiss"), Description("Kisses another member."), Aliases("smooch"), RequireGuild]
        public async Task KissAsync(CommandContext context, [RemainingText, Description("Member to kiss.")] DiscordMember member = null)
        {
            if (member == null)
            {
                await context.RespondAsync("You need to name someone to kiss.");
                return;
            }

            await context.RespondAsync(embed: new DiscordEmbedBuilder().WithTitle($"{context.User.Username} kissed {member.Username}.").WithImageUrl(await NekosLifeImage.GetAsync(NekosLifeImage.Endpoint.Kiss)));
        }

        [Command("pat"), Description("Hugs another member."), Aliases("pet", "headpat"), RequireGuild]
        public async Task PatAsync(CommandContext context, [RemainingText, Description("Member to pat on the head.")] DiscordMember member = null)
        {
            if (member == null)
            {
                await context.RespondAsync("You need to name someone to pat on their head.");
                return;
            }

            await context.RespondAsync(embed: new DiscordEmbedBuilder().WithTitle($"{context.User.Username} pat {member.Username} on their head.").WithImageUrl(await NekosLifeImage.GetAsync(NekosLifeImage.Endpoint.Pat)));
        }

        [Command("slap"), Description("Slaps another member."), Aliases("hit"), RequireGuild]
        public async Task SlapAsync(CommandContext context, [RemainingText, Description("Member to slap.")] DiscordMember member = null)
        {
            if (member == null)
            {
                await context.RespondAsync("You need to name someone to slap.");
                return;
            }

            await context.RespondAsync(embed: new DiscordEmbedBuilder().WithTitle($"{context.User.Username} slapped {member.Username}.").WithImageUrl(await NekosLifeImage.GetAsync(NekosLifeImage.Endpoint.Slap)));
        }

        [Command("why"), Description("Gives you a random question."), Aliases("what")]
        public async Task WhyAsync(CommandContext context)
        {
            string question = await NekosLifeWhyQuestion.GetAsync();
            await context.RespondAsync($"Here's a question for you:\n\n{char.ToUpper(question[0]) + question.Substring(1)}");
        }
    }
}
