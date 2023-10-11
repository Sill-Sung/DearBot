using System;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using DearBot.Data;
using DearBot.LogUtil;
using DearBot.Message;
using DearBot.Modal;
using DearBot.Services;

namespace DearBot
{
    class Program
    {
        private readonly DiscordSocketClient botClient;
        private readonly IConfiguration botConfig;

        private static void Main(string[] args) => new Program().MainAsync().GetAwaiter().GetResult();

        public Program()
        {
            /* Set Initailize Discord Client --------------------------------------------------------------------------------*/
            var _clientConfig = new DiscordSocketConfig
            {
                TotalShards = 5
                , UseInteractionSnowflakeDate = false
                , GatewayIntents = GatewayIntents.AllUnprivileged
                                 | GatewayIntents.MessageContent
                                 | GatewayIntents.All
            };
            botClient = new DiscordSocketClient(_clientConfig);
            botConfig = new ConfigurationBuilder().SetBasePath(Environment.CurrentDirectory).AddJsonFile("config.json").Build();
            /*---------------------------------------------------------------------------------------------------------------*/


            /* Set Event Handler for Discord Client --------------------------------------------------------------------------------*/
            DearLog.SetSocketClient(botClient);
            botClient.Log += DearLog.GeneralLog;
            botClient.Ready += DearLog.Ready;

            botClient.MessageReceived += _client_MessageReceived;
            botClient.ButtonExecuted += _client_ButtonExecuted;
            /*----------------------------------------------------------------------------------------------------------------------*/
        }

        public async Task MainAsync()
        {
            var config = new DiscordSocketConfig
            {
                TotalShards = 2
                , UseInteractionSnowflakeDate = false
                , GatewayIntents = GatewayIntents.AllUnprivileged
                                 | GatewayIntents.MessageContent
                                 | GatewayIntents.All
            };



            await botClient.LoginAsync(TokenType.Bot, botConfig["Token"]);
            await botClient.StartAsync();

            await Task.Delay(Timeout.Infinite);
        }

        private ServiceProvider ConfigureServices(DiscordSocketConfig config)
            => new ServiceCollection()
                .AddSingleton(new DiscordShardedClient(config))
                .AddSingleton<CommandService>()
                .AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordShardedClient>()))
                .AddSingleton<CommandHandlingService>()
                .AddSingleton<InteractionHandlingService>()
                .BuildServiceProvider();

        private async Task _client_MessageReceived(SocketMessage arg)
        {
            if (arg.Author.IsBot)
                return;
            
            SocketUser user = arg.Author;
            SocketGuild guild = user.MutualGuilds.Where(x => x.Channels.Count(x => x.Id == arg.Channel.Id) > 0).First();

            if (arg.Content.Equals("..hello"))
            {
                await arg.Channel.SendMessageAsync("world!");
            }
            else if (arg.Content.Equals("..멈머"))
            {
                await arg.Channel.SendMessageAsync("멈멍!");
            }
            else if (arg.Content.Equals("..냥냐"))
            {
                await arg.Channel.SendMessageAsync("땡깡!");
            }
            else if (arg.Content.Equals("..퇴근시켜줘"))
            {
                await arg.Channel.SendMessageAsync("안돼. 못 가. 못 보내줘.");
            }
            else if (arg.Type == MessageType.GuildMemberJoin || arg.Content.Equals("..test") || arg.Content.Equals("..join"))
            {
                if (arg.Channel is SocketDMChannel)
                    return;

                MessageWelcome messageWelcome = new MessageWelcome(botClient, arg, guild);
                await messageWelcome.SendMessage();
            }

        }

        private async Task _client_ButtonExecuted(SocketMessageComponent arg)
        {
            DataContainer data = new DataContainer(botClient, arg);

            switch (data.Key)
            {
                case "join":
                    ModalInterviewReservation modalInterview = new ModalInterviewReservation(botClient, arg);

                    await modalInterview.ShowReservationModal();

                    break;
                case "customer":
                    MessageCustomer messageCustomer = new MessageCustomer(botClient, arg);

                    await messageCustomer.SendMessageToAdministrator(); // Send [User Selected 'Customer'] Message to Administrators
                    await messageCustomer.SendMessage();                // Send Welcome Message to User

                    break;
                case "yes":
                    MessageClear messageClear = new MessageClear(botClient, arg.Message, data.Guild);

                    if (arg.Message.Channel is SocketDMChannel)
                        await messageClear.ClearDMMessage(arg);
                    else
                        await messageClear.ClearMessage(arg);

                    break;
                case "no":
                    await arg.Message.DeleteAsync();   // Delete Previus Message (Select Purpose)
                    break;
            }
        }
    }
}
