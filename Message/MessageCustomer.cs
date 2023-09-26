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
    internal class MessageCustomer
    {
        private readonly DiscordSocketClient botClient = null;
        private readonly SocketMessageComponent messageComponent = null;
        private readonly DataContainer dataContainer = null;

        public MessageCustomer(DiscordSocketClient arg_bot, SocketMessageComponent arg_messageComponent)
        {
            botClient = arg_bot;
            messageComponent = arg_messageComponent;

            dataContainer = new DataContainer(botClient, messageComponent);
        }
        public async Task SendMessage()
        {
            await Discord.UserExtensions.SendMessageAsync(dataContainer.User, "[손님] 권한이 정상적으로 부여됐습니다!\n"
                                                                             + "필히 서버 공지 사항을 확인해주시고, 향후 클랜 가입을 원하시면 ~~를 이용해주세요!");
        }

        public async Task SendMessageToAdministrator()
        {
            await messageComponent.Message.DeleteAsync();   // Delete Previus Message (Select Purpose)
            await dataContainer.Guild.DownloadUsersAsync(); // Download All Users in Guild

            // Send message to Only Guild's [ Owner & Administrator ] ---------------------------------------------------------------------------------------------------------------
            dataContainer.Guild.Roles.Where(x => x.Permissions.Administrator).ToList()  // Get Administrator Roles
                                     .ForEach(role => dataContainer.Guild.Users.Where(x => x.Roles.Contains(role) || x == dataContainer.Guild.Owner).ToList() // Get Users has Administrator Roles
                                                                               .ForEach(x => Discord.UserExtensions.SendMessageAsync(x, dataContainer.User.AvatarId + "님이 손님으로 입장하셨습니다.").Wait()));
        }
    }
}
