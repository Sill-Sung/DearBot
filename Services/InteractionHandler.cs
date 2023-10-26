using System;
using System.Reflection;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;

using Discord;
using Discord.WebSocket;
using Discord.Interactions;

namespace DearBot.Services
{
    public class InteractionHandler
    {
        private readonly DiscordShardedClient _client;
        private readonly InteractionService _handler;
        private readonly IServiceProvider _services;
        private readonly IConfiguration _configuration;
        public InteractionHandler(DiscordShardedClient client, InteractionService handler, IServiceProvider services, IConfiguration config)
        {
            _client = client;
            _handler = handler;
            _services = services;
            _configuration = config;

            _handler.Log += LogAsync;

            _client.ShardReady += Ready;
            _client.InteractionCreated += HandleInteraction;
        }

        public async Task InitializeAsync()
        {
            _handler.Log += LogAsync;
            _client.ShardReady += Ready;

            await _handler.AddModulesAsync(Assembly.GetEntryAssembly(), _services);

            _client.InteractionCreated += HandleInteraction;
        }

        private async Task LogAsync(LogMessage log) => Console.WriteLine(log);

        private async Task Ready(DiscordSocketClient arg)
        {
            // Context & Slash commands can be automatically registered, but this process needs to happen after the client enters the READY state.
            // Since Global Commands take around 1 hour to register, we should use a test guild to instantly update and test our commands.
            await _handler.RegisterCommandsGloballyAsync(true);
        }

        private async Task HandleInteraction(SocketInteraction interaction)
        {
            try
            {
                IInteractionContext interactionContext;

                if(interaction.Type == InteractionType.MessageComponent)
                {
                    SocketMessageComponent socketMessageComponent = (SocketMessageComponent)interaction;
                    interactionContext = new ShardedInteractionContext<SocketMessageComponent>(_client, socketMessageComponent);
                }
                else
                {
                    interactionContext = new ShardedInteractionContext(_client, interaction);
                }

                // Execute the incoming command.
                var result = await _handler.ExecuteCommandAsync(interactionContext, _services);
                
                if (!result.IsSuccess)
                    switch (result.Error)
                    {
                        case InteractionCommandError.UnmetPrecondition:
                            // implement
                            break;
                        default:
                            break;
                    }
            }
            catch
            {
                // If Slash Command execution fails it is most likely that the original interaction acknowledgement will persist. It is a good idea to delete the original
                // response, or at least let the user know that something went wrong during the command execution.
                if (interaction.Type is InteractionType.ApplicationCommand)
                    await interaction.GetOriginalResponseAsync().ContinueWith(async (msg) => await msg.Result.DeleteAsync());
            }
        }
    }
}

