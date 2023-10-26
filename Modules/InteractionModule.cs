using System;
using System.Text;
using System.Threading.Tasks;

using Discord;
using Discord.Interactions;
using Discord.WebSocket;

using DearBot.Data;
using DearBot.Modal;
using DearBot.Message;
using DearBot.Services;

namespace DearBot.Modules
{
    public class InteractionModule : InteractionModuleBase<ShardedInteractionContext>
    {
        public InteractionService? Commands { get; set; }
        private InteractionHandler _handler;

        public InteractionModule(InteractionHandler handler)
        {
            _handler = handler;
        }

        [SlashCommand("echo", "Repeat the input")]
        public async Task Echo(string echo, [Summary(description: "mention the user")] bool mention = false)
        {
            await RespondAsync(echo + (mention ? Context.User.Mention : string.Empty));
        }

        [ModalInteraction("interview_reservation")]
        public async Task ModalResponse(InterviewModal modal)
        {
            MessageInfo messageInfo = MessageList.GetMessageInfo(((SocketModal)Context.Interaction).Message.Id);
            MessageList.Messages.Add(new MessageInfo(Context.Interaction.Id, messageInfo.GuildID, messageInfo.UserID, messageInfo.MessageID, messageInfo.RootMessageID));

            await modal.AnnounceToGuildAdmin(Context, messageInfo);
            await modal.ResponseInterview(Context, messageInfo);

            await Context.Interaction.RespondAsync();
            await Context.Interaction.DeleteOriginalResponseAsync();

            MessageList.RemoveMessageInfos(messageInfo.RootMessageID);
        }

        [SlashCommand(name: "olduser", description: "장기 미접속 유저 목록을 출력합니다.")]
        public async Task GetClanOldUser(string clan_name, int limit_days)
        {
            if (Context.User.IsBot || Context.Channel is SocketDMChannel)
                return;

            try
            {
                MessageOldUsers messageOldUsers = new MessageOldUsers(Context);
                await messageOldUsers.GetOldUsers(clan_name, limit_days);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        [SlashCommand(name: "olduser_except", description: "특정 권한 유저를 제외한 장기 미접속 유저 목록을 출력합니다.")]
        public async Task GetClanOldUser(string clan_name, int limit_days, string except_role_name)
        {
            if (Context.User.IsBot || Context.Channel is SocketDMChannel || Context.Guild == null)
                return;

            if (Context.Guild.Roles.Where(x=> x.Name.Equals(except_role_name)).ToList().Count() == 0)
            {
                await Context.Interaction.RespondAsync(text: $"해당 디스코드 서버에 [{except_role_name}] 이름을 가진 권한을 찾을 수 없습니다."
                                                       , ephemeral: true);
            }

            try
            {
                MessageOldUsers messageOldUsers = new MessageOldUsers(Context);
                await messageOldUsers.GetOldUsersExcept(clan_name, limit_days, except_role_name);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
