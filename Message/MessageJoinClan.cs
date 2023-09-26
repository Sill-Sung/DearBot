using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Discord;
using Discord.WebSocket;

using DearBot.Data;

namespace DearBot.Message
{
    internal class MessageJoinClan
    {
        private readonly DiscordSocketClient botClient = null;
        private readonly SocketMessageComponent messageComponent = null;
        private readonly DataContainer dataContainer = null;

        public MessageJoinClan(DiscordSocketClient arg_bot, SocketMessageComponent arg_messageComponent)
        {
            botClient = arg_bot;
            messageComponent = arg_messageComponent;

            dataContainer = new DataContainer(botClient, messageComponent);
        }
        public async Task SendMessage()
        {
            await Discord.UserExtensions.SendMessageAsync(dataContainer.User, "정상적으로 클랜 가입 신청 요청이 전송되었습니다!\n"
                                                                             + "저희 관리자가, 입력 내용을 기반으로 클랜 가입을 도와드릴거에요!\n"
                                                                             + "별도의 문의 사항이 있다면, 클랜 내 관리자에게 DM을 전송해주세요!");
        }

        public async Task SendMessageToAdministrator()
        {
            StringBuilder sb_message = new StringBuilder();

            sb_message.AppendFormat("{0}({1})님이 클랜 가입 신청하셨습니다.", dataContainer.User.Mention, dataContainer.User.Username);

            await messageComponent.Message.DeleteAsync();   // Delete Previus Message (Select Purpose)
            await dataContainer.Guild.DownloadUsersAsync(); // Download All Users in Guild

            // Send message to Only Guild's [ Owner & Administrator ] ---------------------------------------------------------------------------------------------------------------
            dataContainer.Guild.Roles.Where(x => x.Permissions.Administrator).ToList()  // Get Administrator Roles
                                     .ForEach(role => dataContainer.Guild.Users.Where(x => x.Roles.Contains(role) || x == dataContainer.Guild.Owner).ToList() // Get Users has Administrator Roles
                                                                               .ForEach(x => Discord.UserExtensions.SendMessageAsync(x, sb_message.ToString()).Wait()));
        }
    }
}
