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
            this.botClient = arg_bot;

            this.messageComponent = arg_messageComponent;
            this.dataContainer = new DataContainer(botClient, arg_messageComponent);
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
        

        public async Task SendMessageAsk()
        {
            ComponentBuilder component = new ComponentBuilder();
            EmbedBuilder eBuilder = new Discord.EmbedBuilder();

            // Message Author ----------------------------------------------------------------
            EmbedAuthorBuilder authorBuilder = new EmbedAuthorBuilder();
            authorBuilder.Name = string.Format("{0}", dataContainer.Guild.Name);
            authorBuilder.IconUrl = dataContainer.Guild.IconUrl != null ? dataContainer.Guild.IconUrl : string.Empty;

            // Message Title -----------------------------------------------------------------
            StringBuilder sb_embedTitle = new StringBuilder();
            sb_embedTitle.AppendLine("클랜 가입을 위한 간단한 면접이 있어요!");

            // Message Description -----------------------------------------------------------
            StringBuilder sb_embedDesc = new StringBuilder();
            sb_embedDesc.Append("클랜 가입을 위해, 클랜 관리자와 간단한 면접이 있을 예정이에요!").Append(Environment.NewLine);
            sb_embedDesc.Append("소요 시간은 대략 5분 내외예요.").Append(Environment.NewLine);
            sb_embedDesc.Append("면접이 가능한 편한 일자 및 시간대를 입력해주시면, 관리자들에게 전달해둘게요!").Append(Environment.NewLine);

            // Message Footer ----------------------------------------------------------------
            EmbedFooterBuilder footerBuilder = new EmbedFooterBuilder();
            footerBuilder.WithIconUrl(dataContainer.Guild.IconUrl)
                         .WithText("Created at : " + messageComponent.CreatedAt.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss") + " Created by DearBot. Made by Dearest");

            // Set EmbedBuilder --------------------------------------------------------------
            eBuilder.WithAuthor(authorBuilder)
                    .WithTitle(sb_embedTitle.ToString())
                    .WithDescription(sb_embedDesc.ToString())
                    .WithFooter(footerBuilder);

            // Message Menu Options  ----------------------------------------------------------------
            ActionRowBuilder actionRowBuilder = new ActionRowBuilder();
            SelectMenuBuilder selectMenu = new SelectMenuBuilder();
            selectMenu.WithPlaceholder("일자 선택");
            selectMenu.WithCustomId("day");
            selectMenu.WithMinValues(1);
            selectMenu.WithMaxValues(1);
            selectMenu.AddOption(new SelectMenuOptionBuilder().WithLabel("오늘").WithValue("today"));
            selectMenu.AddOption(new SelectMenuOptionBuilder().WithLabel("내일").WithValue("tomorrow"));
            selectMenu.AddOption(new SelectMenuOptionBuilder().WithLabel("이번 주").WithValue("this_week"));
            selectMenu.AddOption(new SelectMenuOptionBuilder().WithLabel("기타").WithValue("other"));

            actionRowBuilder.WithSelectMenu(selectMenu);
            component.AddRow(actionRowBuilder);
            //component.WithSelectMenu(selectMenu);
            //actionRow.WithSelectMenu(selectMenu);   // Set Menu Row
            //component.AddRow(actionRow);            // Add Menu Row to Componet

            await messageComponent.Message.DeleteAsync();   // Delete Previus Message (Select Purpose)
            await Discord.UserExtensions.SendMessageAsync(user: dataContainer.User
                                          , text: string.Empty
                                          , isTTS: false
                                          , embed: eBuilder.Build()
                                          , options: null
                                          , allowedMentions: null
                                          , components: component.Build()
                                          , embeds: null);
        }
    }
}
