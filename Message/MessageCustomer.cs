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
        public Task SendMessage()
        {
            messageComponent.Message.DeleteAsync();   // Delete Previus Message (Select Purpose)

            EmbedBuilder embed = new EmbedBuilder();

            /* Message Author ----------------------------------------------------------------*/
            EmbedAuthorBuilder authorBuilder = new EmbedAuthorBuilder();
            authorBuilder.Name = string.Format("{0}", dataContainer.Guild.Name);
            authorBuilder.IconUrl = dataContainer.Guild.IconUrl != null ? dataContainer.Guild.IconUrl : string.Empty;

            /* Message Title -----------------------------------------------------------------*/
            StringBuilder sb_embedTitle = new StringBuilder();
            sb_embedTitle.Append("환영합니다!");

            /* Message Description -----------------------------------------------------------*/
            StringBuilder sb_embedDesc = new StringBuilder();
            sb_embedDesc.Append($"[손님] 권한이 정상적으로 부여됐습니다!").Append(Environment.NewLine);
            sb_embedDesc.Append($"필히 서버 공지 사항을 확인해주시고, 클랜 가입을 원하시면 클랜 관리자분들께 연락해주세요!").Append(Environment.NewLine).Append(Environment.NewLine);
            sb_embedDesc.Append("< 관리자 목록 >").Append(Environment.NewLine);

            Dictionary<string, string> dic_adminUsers = new Dictionary<string, string>();
            List<SocketRole> adminRoles = dataContainer.Guild.Roles.Where(x => x.Permissions.Administrator).ToList();

            dic_adminUsers.Add(dataContainer.Guild.Owner.Mention, dataContainer.Guild.Owner.Username);
            foreach (SocketRole role in adminRoles)
            {
                foreach (SocketUser user in role.Members)
                {
                    if (!dic_adminUsers.ContainsKey(user.Mention) && user.IsBot == false)
                    {
                        dic_adminUsers.Add(user.Mention, user.Username);

                    }
                        
                }
            }
            dic_adminUsers.ToList().ToList().ForEach(x => sb_embedDesc.Append($"  {x.Key}({x.Value})").Append(Environment.NewLine));

            /* Message Footer ----------------------------------------------------------------*/
            EmbedFooterBuilder footerBuilder = new EmbedFooterBuilder();
            footerBuilder.WithIconUrl(dataContainer.Guild.IconUrl)
                         .WithText("Created at : " + messageComponent.CreatedAt.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss") + " Created by DearBot. Made by Dearest");

            /* Set EmbedBuilder --------------------------------------------------------------*/
            embed.WithAuthor(authorBuilder)
                    .WithTitle(sb_embedTitle.ToString())
                    .WithDescription(sb_embedDesc.ToString())
                    .WithFooter(footerBuilder);

            dataContainer.Guild.GetUser(dataContainer.User.Id).AddRoleAsync(dataContainer.Guild.GetRole(905007777494216744));
            Discord.UserExtensions.SendMessageAsync(user: dataContainer.User, embed: embed.Build());

            return Task.CompletedTask;
        }

        public Task SendMessageToAdministrator()
        {
            dataContainer.Guild.DownloadUsersAsync(); // Download All Users in Guild

            EmbedBuilder embed = new EmbedBuilder();

            /* Message Author ----------------------------------------------------------------*/
            EmbedAuthorBuilder authorBuilder = new EmbedAuthorBuilder();
            authorBuilder.Name = string.Format("{0}", dataContainer.Guild.Name);
            authorBuilder.IconUrl = dataContainer.Guild.IconUrl != null ? dataContainer.Guild.IconUrl : string.Empty;

            /* Message Title -----------------------------------------------------------------*/
            StringBuilder sb_embedTitle = new StringBuilder();
            sb_embedTitle.Append("클랜에 손님이 오셨습니다.");

            /* Message Description -----------------------------------------------------------*/
            StringBuilder sb_embedDesc = new StringBuilder();
            sb_embedDesc.Append($"{dataContainer.User.Mention}({dataContainer.User.Username})님이 손님으로 입장하셨습니다.");

            /* Message Footer ----------------------------------------------------------------*/
            EmbedFooterBuilder footerBuilder = new EmbedFooterBuilder();
            footerBuilder.WithIconUrl(dataContainer.Guild.IconUrl)
                         .WithText("Created at : " + messageComponent.CreatedAt.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss") + " Created by DearBot. Made by Dearest");

            /* Set EmbedBuilder --------------------------------------------------------------*/
            embed.WithAuthor(authorBuilder)
                    .WithTitle(sb_embedTitle.ToString())
                    .WithDescription(sb_embedDesc.ToString())
                    .WithFooter(footerBuilder);

            // Send message to Only Guild's [ Owner & Administrator ] ---------------------------------------------------------------------------------------------------------------
            Dictionary<string, string> dic_adminUsers = new Dictionary<string, string>();
            List<SocketRole> adminRoles = dataContainer.Guild.Roles.Where(x => x.Permissions.Administrator).ToList();

            dic_adminUsers.Add(dataContainer.Guild.Owner.Mention, dataContainer.Guild.Owner.Username);
            foreach (SocketRole role in adminRoles)
            {
                foreach (SocketUser user in role.Members)
                {
                    if (!dic_adminUsers.ContainsKey(user.Mention) && user.IsBot == false)
                        Discord.UserExtensions.SendMessageAsync(user: user
                                                                , text: string.Empty
                                                                , isTTS: false
                                                                , embed: embed.Build()
                                                                , options: null
                                                                , allowedMentions: null
                                                                , components: null).Wait();
                }
            }

            return Task.CompletedTask;
        }
    }
}
