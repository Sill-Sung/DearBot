using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace DearBot.Services
{
    public class CommandHandlingService
    {
        private readonly CommandService _commandServices;
        private readonly DiscordShardedClient _shardClient;
        private readonly IServiceProvider _provider;

        public CommandHandlingService(IServiceProvider provider)
        {
            _commandServices = provider.GetRequiredService<CommandService>();
            _shardClient = provider.GetRequiredService<DiscordShardedClient>();
            _provider = provider;

            _commandServices.CommandExecuted += CommandExecutedAsync;
            _commandServices.Log += LogAsync;

            _shardClient.MessageReceived += MessageReceivedAsync;
        }

        public async Task InitializeAsync()
        {
            await _commandServices.AddModulesAsync(Assembly.GetEntryAssembly(), _provider);
        }

        public async Task MessageReceivedAsync(SocketMessage message)
        {
            if (message.Author.IsBot) return;

            int commandPos = 0;

            SocketUserMessage userMessage = message as SocketUserMessage;

            if (userMessage == null || userMessage.HasStringPrefix("..", ref commandPos) == false || userMessage.HasMentionPrefix(_shardClient.CurrentUser, ref commandPos))
                return;

            await _commandServices.ExecuteAsync(context: new ShardedCommandContext(_shardClient, userMessage), argPos: commandPos, services: null);
        }
        
        public async Task CommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            // command is unspecified when there was a search failure (command not found); we don't care about these errors
            if (!command.IsSpecified)
                return;

            // the command was successful, we don't care about this result, unless we want to log that a command succeeded.
            if (result.IsSuccess)
                return;

            // the command failed, let's notify the user that something happened.
            await context.Channel.SendMessageAsync($"error: {result}");
        }

        private Task LogAsync(LogMessage log)
        {
            Console.WriteLine(log.ToString());

            return Task.CompletedTask;
        }
    }
}