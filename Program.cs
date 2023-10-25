using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Discord.Interactions;

using DearBot.Services;

namespace DearBot
{
    class Program
    {
        IConfiguration _appConfig;
        private readonly HttpClient _webClient;
        private readonly string? _apiKey;
        private readonly string? _botToken;
        private readonly DiscordSocketConfig _socketConfig = new DiscordSocketConfig
        {
            TotalShards = 1,
            MessageCacheSize = 100,
            AlwaysDownloadUsers = true,
            UseInteractionSnowflakeDate = false,
            GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent | GatewayIntents.All
        };

        private static void Main(string[] args) => new Program()
                                                        .MainAsync()
                                                        .GetAwaiter()
                                                        .GetResult();

        public Program()
        {
            _appConfig = new ConfigurationBuilder().SetBasePath(Environment.CurrentDirectory).AddJsonFile("config.json").Build();
            _apiKey = _appConfig.GetSection("X-API-Key").Value;
            _botToken = _appConfig.GetSection("Token").Value;

            /* Set Web Client ----------------------------------------------------------------------------------------------------*/
            _webClient = new HttpClient();
            _webClient.DefaultRequestHeaders.Add("X-API-Key", _apiKey);
        }

        public async Task MainAsync()
        {
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("config.json", optional: true)
                .Build();

            using (var services = ConfigureServices(config))
            {
                DiscordShardedClient shardClients = services.GetRequiredService<DiscordShardedClient>();

                await services.GetRequiredService<CommandHandlingService>().InitializeAsync();
                await services.GetRequiredService<InteractionHandler>().InitializeAsync();

                shardClients.ShardReady += ShardClients_ShardReady;
                shardClients.Log += ShardClients_Log;

                //DearLog.SetSocketClient(shardClients, commandServices);

                await shardClients.LoginAsync(TokenType.Bot, _botToken);
                await shardClients.StartAsync();

                CommandService commandServices = services.GetRequiredService<CommandService>();
                InteractionService interactionServices = services.GetRequiredService<InteractionService>();

                await commandServices.AddModulesAsync(assembly: this.GetType().Assembly, services: null);

                await Task.Delay(Timeout.Infinite);
            }
        }

        private ServiceProvider ConfigureServices(IConfiguration config) => new ServiceCollection()
                                                        .AddSingleton(config)
                                                        .AddSingleton(new DiscordShardedClient(_socketConfig))
                                                        .AddSingleton<CommandService>()
                                                        .AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordShardedClient>()))
                                                        .AddSingleton<CommandHandlingService>()
                                                        .AddSingleton<InteractionHandler>()
                                                        .BuildServiceProvider();

        private Task ShardClients_Log(LogMessage log)
        {
            Console.WriteLine(log.ToString());
            return Task.CompletedTask;
        }

        private Task ShardClients_ShardReady(DiscordSocketClient client)
        {
            Console.WriteLine($"{client.CurrentUser} is connected!");

            return Task.CompletedTask;
        }
    }
}
