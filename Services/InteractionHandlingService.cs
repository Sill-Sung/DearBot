using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DearBot.Services
{
    internal class InteractionHandlingService
    {
        private readonly InteractionService _interactionService;
        private readonly DiscordShardedClient _shardClient;
        private readonly IServiceProvider _provider;

        public InteractionHandlingService(IServiceProvider provider)
        {
            _interactionService = provider.GetRequiredService<InteractionService>();
            _shardClient = provider.GetRequiredService<DiscordShardedClient>();
            _provider = provider;

            _interactionService.Log += LogAsync;

            _shardClient.ShardReady += ReadyAsync;
            _shardClient.InteractionCreated += InteractionCreatedAsync;
        }


        // Register all modules, and add the commands from these modules to either guild or globally depending on the build state.
        public async Task InitializeAsync()
        {
            await _interactionService.AddModulesAsync(typeof(InteractionHandlingService).Assembly, _provider);
        }

        private async Task InteractionCreatedAsync(SocketInteraction interaction)
        {
            await Task.Run(async () =>
            {
                var context = new ShardedInteractionContext(_shardClient, interaction);
                await _interactionService.ExecuteCommandAsync(context, _provider);
            });

            await Task.CompletedTask;
        }

        private Task LogAsync(LogMessage log)
        {
            Console.WriteLine(log.ToString());

            return Task.CompletedTask;
        }

        private async Task ReadyAsync(DiscordSocketClient _)
        {
#if DEBUG
            await _interactionService.RegisterCommandsToGuildAsync(1 /* implement */);
#else
            await _interactionService.RegisterCommandsGloballyAsync();
#endif
        }
    }
}