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
        private GarbageCollection garbage;
        
        public static void Main(string[] args)
        {
            Configuration configuration;

            string defaultConfigPath, defaultDatabasePath;

#if DEBUG
            defaultConfigPath    = "../../config.json";
            defaultDatabasePath  = "../../blu-ray.db";
#else
            defaultConfigPath   = "config.json";
            defaultDatabasePath = "blu-ray.db";
#endif

            try { configuration = new Configuration(args.Length > 0 ? args[0] : defaultConfigPath); }
            catch (Exception ex)
            {
                Console.WriteLine($"An error of type \"{ex.GetType().Name}\" occurred: \"{ex.Message}\".");
                return;
            }

            new Program().RunAsync(configuration, args.Length > 1 ? int.Parse(args[1]) : 0, args.Length > 2 ? int.Parse(args[2]) : 1, args.Length > 3 ? args[3] : defaultDatabasePath).GetAwaiter().GetResult();
        }

        internal async Task RunAsync(Configuration configuration, int shardId, int shardCount, string dbPath)
        {
            LogLevel level;
            switch (configuration.LogLevel.ToLower())
            {
                case "debug":
                    level = LogLevel.Debug;
                    break;

                case "warning":
                    level = LogLevel.Warning;
                    break;

                case "error":
                    level = LogLevel.Error;
                    break;

                case "critical":
                    level = LogLevel.Critical;
                    break;

                default:
                    level = LogLevel.Info;
                    break;
            }

            client = new DiscordClient(new DiscordConfiguration
            {
                AutoReconnect = true,
                LogLevel = level,
                Token = configuration.Token,
                TokenType = TokenType.Bot,
                ShardCount = shardCount,
                ShardId = shardId,
                UseInternalLogHandler = true
            });

            Events.supportGuildId = configuration.SupportId;

            client.Ready += async (ReadyEventArgs e) =>
            {
                e.Client.DebugLogger.LogMessage(LogLevel.Info, "Client", $"The client is now ready. Connected as {e.Client.CurrentUser.Username}#{e.Client.CurrentUser.Discriminator} (ID: {e.Client.CurrentUser.Id}).", DateTime.Now);
                await e.Client.UpdateStatusAsync(new DiscordActivity(configuration.Status.Name, configuration.Status.Type));
            };

            client.Resumed                      += Events.OnClientResumed;
            client.ClientErrored                += Events.OnClientError;
            client.SocketErrored                += Events.OnClientSocketError;
            client.GuildAvailable               += Events.OnGuildJoin;
            client.GuildCreated                 += Events.OnGuildJoin;
            client.GuildDeleted                 += Events.OnGuildLeave;

            client.MessageCreated               += AFK.AFKMessageHandler;

            client.MessageReactionAdded         += Starboard.ReactionAddHandler;
            client.MessageReactionRemoved       += Starboard.ReactionRemoveHandler;
            client.MessageReactionRemovedEmoji  += Starboard.ReactionRemoveEmojiHandler;
            client.MessageReactionsCleared      += Starboard.ReactionRemoveAllHandler;

            commands = client.UseCommandsNext(new CommandsNextConfiguration
            {
                CaseSensitive = true,
                EnableDefaultHelp = true,
                EnableDms = true,
                EnableMentionPrefix = true,
                IgnoreExtraArguments = true,
                StringPrefixes = configuration.Prefixes,
            });

            commands.CommandExecuted    += Events.OnCommandExecute;
            commands.CommandErrored     += Events.OnCommandError;

            commands.RegisterCommands<Fun>();
            commands.RegisterCommands<Info>();
            commands.RegisterCommands<Moderation>();
            commands.RegisterCommands<NSFW>();
            commands.RegisterCommands<Owner>();
            commands.RegisterCommands<Search>();
            commands.RegisterCommands<Settings>();

            client.UseInteractivity(new InteractivityConfiguration
            {
                PaginationBehaviour = PaginationBehaviour.Ignore,
                PaginationDeletion = PaginationDeletion.DeleteEmojis,
                PollBehaviour = PollBehaviour.DeleteEmojis
            });

            IMDb.apiKey = configuration.OMDb;
            Managers.Google.InitializeService(configuration.Google.Key, configuration.Google.Cx);
            await Spotify.AuthorizeAsync(configuration.Spotify.ID, configuration.Spotify.Secret, client.DebugLogger);
            await Database.ConnectAsync(dbPath, client.DebugLogger);
            await client.ConnectAsync();

            garbage = new GarbageCollection();

            AppDomain.CurrentDomain.ProcessExit += new EventHandler((s, e) => HandleProcessQuit().GetAwaiter().GetResult());            
            await Task.Delay(-1);
        }

        internal async Task HandleProcessQuit()
        {
            await client.DisconnectAsync();
            Database.Disconnect();
            garbage.Dispose();
        }
    }
}
