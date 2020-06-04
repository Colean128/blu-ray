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

using Bot.Commands;
using Bot.Managers;
using Bot.Structures;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using System;
using System.Threading.Tasks;

namespace Bot
{
    public class Program
    {
        private DiscordClient client;
        private CommandsNextExtension commands;
        private InteractivityExtension interactivity;
        
        public static void Main(string[] args)
        {
            Configuration configuration;

            try { configuration = new Configuration(args.Length > 0 ? args[0] : "config.json"); }
            catch (Exception ex)
            {
                Console.WriteLine($"An error of type \"{ex.GetType().Name}\" occurred: \"{ex.Message}\".");
                return;
            }

            new Program().RunAsync(configuration).GetAwaiter().GetResult();
        }

        internal async Task RunAsync(Configuration configuration)
        {
            LogLevel level;
            switch (configuration.LogLevel)
            {
            case "Debug":
                level = LogLevel.Debug;
                break;

            case "Info":
                level = LogLevel.Info;
                break;

            case "Warning":
                level = LogLevel.Warning;
                break;

            case "Error":
                level = LogLevel.Error;
                break;

            default:
                level = LogLevel.Critical;
                break;
            }

            client = new DiscordClient(new DiscordConfiguration
            {
                AutoReconnect = true,
                LogLevel = level,
                Token = configuration.Token,
                TokenType = TokenType.Bot,
                ShardCount = 1,
                ShardId = 0,
                UseInternalLogHandler = true
            });

            client.Ready += OnClientReady;
            client.Resumed += OnClientReady;

            client.ClientErrored += OnClientError;
            client.SocketErrored += OnClientSocketError;

            client.GuildAvailable += OnGuildJoin;
            client.GuildCreated += OnGuildJoin;

            client.GuildDeleted += OnGuildLeave;

            commands = client.UseCommandsNext(new CommandsNextConfiguration
            {
                CaseSensitive = true,
                EnableDefaultHelp = true,
                EnableDms = true,
                EnableMentionPrefix = true,
                IgnoreExtraArguments = true,
                StringPrefixes = configuration.Prefixes,
            });

            commands.CommandExecuted += OnCommandExecute;
            commands.CommandErrored += OnCommandError;

            commands.RegisterCommands<Fun>();
            commands.RegisterCommands<Info>();
            commands.RegisterCommands<NSFW>();
            commands.RegisterCommands<Owner>();
            commands.RegisterCommands<Search>();

            interactivity = client.UseInteractivity(new InteractivityConfiguration
            {
                PaginationBehaviour = PaginationBehaviour.Ignore,
                PaginationDeletion = PaginationDeletion.DeleteEmojis,
                PollBehaviour = PollBehaviour.DeleteEmojis
            });

            await Spotify.AuthorizeAsync(configuration.Spotify.ID, configuration.Spotify.Secret, client.DebugLogger);
            await client.ConnectAsync();
            await client.UpdateStatusAsync(new DiscordActivity(configuration.Prefixes[0]+"help", ActivityType.ListeningTo));
            await Task.Delay(-1);
        }

        private Task OnClientReady(ReadyEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "Client", $"The client is now ready. Connected as {e.Client.CurrentUser.Username}#{e.Client.CurrentUser.Discriminator} (ID: {e.Client.CurrentUser.Id}).", DateTime.Now);
            return Task.CompletedTask;
        }

        private Task OnClientError(ClientErrorEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(LogLevel.Error, "Client", $"The client encountered an error.", DateTime.Now, e.Exception);
            return Task.CompletedTask;
        }

        private Task OnClientSocketError(SocketErrorEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(LogLevel.Error, "Client", $"The client's connection encountered an error.", DateTime.Now, e.Exception);
            return Task.CompletedTask;
        }

        private Task OnGuildJoin(GuildCreateEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "Guilds", $"Joined guild \"{e.Guild.Name}\" (ID: {e.Guild.Id}).", DateTime.Now);
            return Task.CompletedTask;
        }

        private Task OnGuildLeave(GuildDeleteEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "Guilds", $"Left guild \"{e.Guild.Name}\" (ID: {e.Guild.Id}).", DateTime.Now);
            return Task.CompletedTask;
        }

        private Task OnCommandExecute(CommandExecutionEventArgs e)
        {
            e.Context.Client.DebugLogger.LogMessage(LogLevel.Info, "Commands", $"User {e.Context.User.Username}#{e.Context.User.Discriminator} (ID: {e.Context.User.Id}) executed command \"{e.Command.Name}\" in channel \"{e.Context.Channel.Id}\".", DateTime.Now);
            return Task.CompletedTask;
        }

        private Task OnCommandError(CommandErrorEventArgs e)
        {
            if (e.Command == null) return Task.CompletedTask;

            e.Context.Client.DebugLogger.LogMessage(LogLevel.Error, "Commands", $"The command \"{e.Command.Name}\" executed by the user {e.Context.User.Username}#{e.Context.User.Discriminator} (ID: {e.Context.User.Id}) in channel \"{e.Context.Channel.Id}\" encountered an error.", DateTime.Now, e.Exception);
            return Task.CompletedTask;
        }
    }
}
