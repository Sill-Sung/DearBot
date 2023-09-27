using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Discord;
using Discord.WebSocket;

namespace DearBot.DearLog
{
    public static class DearLog
    {
        private static DiscordSocketClient? botClient;

        public static void SetSocketClient(DiscordSocketClient arg_client) => botClient = arg_client;

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

        private static async Task MessageUpdated(Cacheable<IMessage, ulong> arg1, SocketMessage arg2, ISocketMessageChannel arg3)
        {
            var message = await arg1.GetOrDownloadAsync();

            Console.WriteLine($"{message} -> {arg2}");
        }
    }
}
