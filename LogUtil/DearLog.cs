using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace DearBot.LogUtil
{
    public static class DearLog
    {
        private static DiscordSocketClient? botClient;
        private static DiscordShardedClient? botSClient;
        private static CommandService? commandService;

        public static void SetSocketClient(DiscordSocketClient arg_client, CommandService arg_commandService)
        {
            botClient = arg_client;
            commandService = arg_commandService;

            botClient.Log += GeneralLog;
            botClient.Ready += Ready;
            commandService.Log += GeneralLog;
        }

        public static void SetSocketClient(DiscordShardedClient arg_client, CommandService arg_commandService)
        {
            botSClient = arg_client;
            commandService = arg_commandService;

            botSClient.Log += GeneralLog;
            botSClient.ShardReady += BotSClient_ShardReady;
            commandService.Log += GeneralLog;
        }

        public static Task GeneralLog(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

        public static Task Ready()
        {
            Console.WriteLine($"{botClient.CurrentUser} is connected!");

            return Task.CompletedTask;
        }

        private static Task BotSClient_ShardReady(DiscordSocketClient arg)
        {
            Console.WriteLine($"{arg.CurrentUser} is connected!");

            return Task.CompletedTask;
        }

        private static async Task MessageUpdated(Cacheable<IMessage, ulong> arg1, SocketMessage arg2, ISocketMessageChannel arg3)
        {
            var message = await arg1.GetOrDownloadAsync();

            Console.WriteLine($"{message} -> {arg2}");
        }
    }
}
