using System;
using System.Text;
using System.Threading.Tasks;

using Discord;
using Discord.Interactions;
using Discord.WebSocket;

using DearBot.Data;
using DearBot.Modal;
using DearBot.Services;

namespace DearBot.Modules
{
    public class InteractionModule : InteractionModuleBase<ShardedInteractionContext>
    {
        // Dependencies can be accessed through Property injection, public properties with public setters will be set by the service provider
        public InteractionService? Commands { get; set; }
        private InteractionHandler _handler;


        // Constructor injection is also a valid way to access the dependencies
        public InteractionModule(InteractionHandler handler)
        {
            _handler = handler;
        }

        [ModalInteraction("interview_reservation")]
        public async Task ModalResponse(InterviewModal modal)
        {
            MessageInfo messageInfo = MessageList.GetMessageInfo(((SocketModal)Context.Interaction).Message.Id);
            MessageList.Messages.Add(new MessageInfo(Context.Interaction.Id, messageInfo.GuildID, messageInfo.UserID, messageInfo.MessageID, messageInfo.RootMessageID));

            await Context.Interaction.RespondAsync();
            await Context.Interaction.DeleteOriginalResponseAsync();

            await modal.AnnounceToGuildAdmin(Context, messageInfo);
            await modal.ResponseInterview(Context, messageInfo);

            MessageList.RemoveMessageInfos(messageInfo.RootMessageID);
        }
    }
}
