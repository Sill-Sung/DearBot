using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;
using Discord.Net;
using Discord.Webhook;
using Discord.WebSocket;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

using DearBot.Data;
using DearBot.Log;
using DearBot.Message;

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
                GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent | GatewayIntents.All
                , UseInteractionSnowflakeDate = false
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
            /*---------------------------------------------------------------------------------------------------------------*/
        }

        public async Task MainAsync()
        {
            await botClient.LoginAsync(TokenType.Bot, botConfig["Token"]);
            await botClient.StartAsync();

            await Task.Delay(Timeout.Infinite);
        }

        private async Task _client_MessageReceived(SocketMessage arg)
        {
            SocketUser user = arg.Author;
            SocketGuild guild = user.MutualGuilds.First();

            if (arg.Author.IsBot)
                return;

            if (arg.Content.Equals(".hello"))
            {
                await arg.Channel.SendMessageAsync("world!");
            }
            else if (arg.Type == MessageType.GuildMemberJoin)
            {
                MessageWelcome messageWelcome = new MessageWelcome(botClient, arg, guild);
                await messageWelcome.SendMessage();
            }
            if (arg.Content.Equals("test"))
            {
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
                    MessageJoinClan messageJoinClan = new MessageJoinClan(botClient, arg);

                    await messageJoinClan.SendMessageToAdministrator(); // Send [User Selected 'Customer'] Message to Administrators
                    await messageJoinClan.SendMessage();                // Send Welcome Message to User
                    break;
                case "customer":
                    MessageCustomer messageCustomer = new MessageCustomer(botClient, arg);

                    await messageCustomer.SendMessageToAdministrator(); // Send [User Selected 'Customer'] Message to Administrators
                    await messageCustomer.SendMessage();                // Send Welcome Message to User

                    break;
            }
        }

        
    }
}
