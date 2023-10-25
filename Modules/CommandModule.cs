using Discord.Commands;
using Discord.WebSocket;

namespace DearBot.Modules
{
    public class CommandModule : ModuleBase<ShardedCommandContext>
    {
        [Command("Hello")]
        public async Task Hello()
        {
            await Context.Channel.SendMessageAsync("world!");
        }

        [Command("멈머")]
        public async Task Bark()
        {
            await Context.Channel.SendMessageAsync("멈멍!");
        }

        [Command("냥냐")]
        public async Task Meow()
        {
            await Context.Channel.SendMessageAsync("땡깡!");
        }

        [Command("clear")]
        public async Task ClearDM()
        {
            if (Context.Channel is SocketDMChannel)
                return;

            DearBot.Message.MessageClear messageClear = new Message.MessageClear(Context.Client, Context.Message, Context.Guild);
            await messageClear.SendMessage();
        }

        [Command("test")]
        public async Task test()
        {
            if (Context.Channel is SocketDMChannel)
                return;

            DearBot.Message.MessageWelcome messageWelcome = new DearBot.Message.MessageWelcome(Context.Client, Context.Message, Context.Guild);
            await messageWelcome.SendMessage();
        }
    }
}
