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
using DearBot.LogUtil;
using DearBot.Message;
using DearBot.Modal;

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
                UseInteractionSnowflakeDate = false
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
            await botClient.LoginAsync(TokenType.Bot, botConfig["Token"]);
            await botClient.StartAsync();

            await Task.Delay(Timeout.Infinite);
        }

        private async Task _client_MessageReceived(SocketMessage arg)
        {
            if (arg.Author.IsBot)
                return;


            SocketUser user = arg.Author;
            SocketGuild guild = user.MutualGuilds.First();


            if (arg.Content.Equals("hello"))
            {
                await arg.Channel.SendMessageAsync("world!");
            }
            else if (arg.Content.Equals("멈머"))
            {
                await arg.Channel.SendMessageAsync("멈멍!");
            }
            else if (arg.Type == MessageType.GuildMemberJoin || arg.Content.Equals("test"))
            {
                if (arg.Channel is SocketDMChannel)
                    return;

                MessageWelcome messageWelcome = new MessageWelcome(botClient, arg, guild);
                await messageWelcome.SendMessage();
            }
            else if (arg.Content.Equals("clear"))
            {
                if (arg.Channel is SocketDMChannel)
                {
                    MessageClear messageClear = new MessageClear(botClient, arg, guild);
                    await messageClear.SendMessage();
                }
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
