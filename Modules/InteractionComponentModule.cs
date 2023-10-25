using System;
using System.Threading.Tasks;

using Discord;
using Discord.WebSocket;
using Discord.Interactions;

using DearBot.Modal;
using DearBot.Services;

namespace DearBot.Modules
{
    public class InteractionComponentModule : InteractionModuleBase<ShardedInteractionContext<SocketMessageComponent>>
    {
        public InteractionService? Commands { get; set; }
        private InteractionHandler _handler;

        public InteractionComponentModule(InteractionHandler handler)
        {
            _handler = handler;
        }

        [ComponentInteraction("join")]
        public async Task ButtonJoinPress()
        {
            await Context.Interaction.RespondWithModalAsync<InterviewModal>("interview_reservation");
        }

        [ComponentInteraction("clear_yes")]
        public async Task ButtonClearYesPress()
        {
            IDMChannel channel = Context.User.CreateDMChannelAsync().Result;

            List<IMessage> messages = await channel.GetMessagesAsync().Flatten().Where(x => x.Author.IsBot).ToListAsync();

            foreach (IMessage message in messages)
            {
                await message.DeleteAsync();
            }

            if ((await Context.Interaction.Channel.GetMessagesAsync().Flatten().Where(x => x.Author.IsBot).AnyAsync()) == true)
                await ButtonClearYesPress();
        }
    }
}
