using Discord.Commands;
using Discord.WebSocket;

using DearBot.Message;

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
            if (Context.Channel is SocketDMChannel && Context.User.IsBot)
                return;

            if (Context.Guild.Users.Where(x => x.Id == Context.User.Id).ToList()
                                   .Where(x => x.GuildPermissions.Has(Discord.GuildPermission.Administrator)).ToList().Count == 0)
                return;

            MessageClear messageClear = new MessageClear(Context.Client, Context.Message, Context.Guild);
            await messageClear.SendMessage();
            await Context.Message.DeleteAsync();
        }

        [Command("test")]
        public async Task test()
        {
            if (Context.Channel is SocketDMChannel && Context.User.IsBot)
                return;

            MessageWelcome messageWelcome = new MessageWelcome(Context.Client, Context.Message, Context.Guild);
            await messageWelcome.SendMessage();
        }
    }
}
