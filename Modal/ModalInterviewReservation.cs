using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Discord;
using Discord.WebSocket;

using DearBot.Data;

namespace DearBot.Modal
{
    internal class ModalInterviewReservation
    {
        private readonly DiscordSocketClient botClient = null;
        private readonly SocketMessageComponent messageComponent = null;
        private readonly DataContainer dataContainer = null;


        public ModalInterviewReservation(DiscordSocketClient arg_bot, SocketMessageComponent arg_messageComponent)
        {
            this.botClient = arg_bot;

            this.messageComponent = arg_messageComponent;
            this.dataContainer = new DataContainer(botClient, arg_messageComponent);
        }

        public async Task ShowReservationModal()
        {
            ModalBuilder modal = new ModalBuilder();
            TextInputBuilder text_reserve_day = new TextInputBuilder();
            TextInputBuilder text_reserve_comment = new TextInputBuilder();

            text_reserve_day.WithLabel("면접 가능 일자");
            text_reserve_day.WithPlaceholder("오늘 오후 8시 / 아무때나 / ...");
            text_reserve_day.WithCustomId("join-reservation-day");
            text_reserve_day.WithStyle(TextInputStyle.Short);
            text_reserve_day.WithMaxLength(400);

            text_reserve_comment.WithLabel("면접 참고 사항");
            text_reserve_comment.WithPlaceholder("비고");
            text_reserve_comment.WithCustomId("join-reservation-comment");
            text_reserve_comment.WithStyle(TextInputStyle.Paragraph);
            text_reserve_comment.WithMaxLength(1000);


            modal.WithTitle("면접 일자");
            modal.WithCustomId("join-reservation-interview");
            modal.AddTextInput(text_reserve_day);
            modal.AddTextInput(text_reserve_comment);

            botClient.ModalSubmitted += ModalSubmitted;

            await messageComponent.RespondWithModalAsync(modal.Build());
        }

        private Task ModalSubmitted(SocketModal arg)
        {
            botClient.ModalSubmitted -= ModalSubmitted;

            arg.RespondAsync();             // Delete Previus Message (Select Purpose)

            SendMessageToAdministrator(arg);

            arg.Message.DeleteAsync();      // Download All Users in Guild

            return Discord.UserExtensions.SendMessageAsync(arg.User, "정상적으로 클랜 가입 신청 요청이 전송되었습니다!\n"
                                                                   + "저희 관리자가, 입력 내용을 기반으로 클랜 가입을 도와드릴거에요!\n"
                                                                   + "별도의 문의 사항이 있다면, 클랜 내 관리자에게 DM을 전송해주세요!");
        }

        public void SendMessageToAdministrator(SocketModal arg_socketModal)
        {
            List<SocketMessageComponentData> data = arg_socketModal.Data.Components.ToList();

            EmbedBuilder embed = new EmbedBuilder();
            
            /* Message Author ----------------------------------------------------------------*/
            EmbedAuthorBuilder authorBuilder = new EmbedAuthorBuilder();
            authorBuilder.Name = string.Format("{0}", dataContainer.Guild.Name);
            authorBuilder.IconUrl = dataContainer.Guild.IconUrl != null ? dataContainer.Guild.IconUrl : string.Empty;

            /* Message Title -----------------------------------------------------------------*/
            StringBuilder sb_embedTitle = new StringBuilder();
            sb_embedTitle.Append("클랜 가입 신청 요청이 있습니다.");

            /* Message Description -----------------------------------------------------------*/
            StringBuilder sb_embedDesc = new StringBuilder();
            sb_embedDesc.Append($"{dataContainer.User.Mention}({dataContainer.User.Username})님이 클랜 가입 신청하셨습니다.");

            /* Message Fields ----------------------------------------------------------------*/
            EmbedFieldBuilder fBuild_day = new EmbedFieldBuilder();
            fBuild_day.WithName("[면접 가능 시간]")
                       .WithValue(data.First(x => x.CustomId == "join-reservation-day").Value)
                       .WithIsInline(false);

            EmbedFieldBuilder fBuild_comment = new EmbedFieldBuilder();
            fBuild_comment.WithName("[면접 참고 사항]")
                       .WithValue(data.First(x => x.CustomId == "join-reservation-comment").Value)
                       .WithIsInline(false);

            /* Message Footer ----------------------------------------------------------------*/
            EmbedFooterBuilder footerBuilder = new EmbedFooterBuilder();
            footerBuilder.WithIconUrl(dataContainer.Guild.IconUrl)
                         .WithText("Created at : " + arg_socketModal.CreatedAt.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss") + " Created by DearBot. Made by Dearest");

            /* Set EmbedBuilder --------------------------------------------------------------*/
            embed.WithAuthor(authorBuilder)
                    .WithTitle(sb_embedTitle.ToString())
                    .WithDescription(sb_embedDesc.ToString())
                    .AddField(fBuild_day)
                    .AddField(fBuild_comment)
                    .WithFooter(footerBuilder);

            // Send message to Only Guild's [ Owner & Administrator ] ---------------------------------------------------------------------------------------------------------------
            dataContainer.Guild.Roles.Where(x => x.Permissions.Administrator).ToList()  // Get Administrator Roles
                                     .ForEach(role => dataContainer.Guild.Users.Where(x => x.Roles.Contains(role) || x == dataContainer.Guild.Owner).ToList() // Get Users has Administrator Roles
                                                                               .ForEach(x => Discord.UserExtensions.SendMessageAsync(user: x
                                                                                                                                     , text : string.Empty
                                                                                                                                     , isTTS: false
                                                                                                                                     , embed: embed.Build()
                                                                                                                                     , options: null
                                                                                                                                     , allowedMentions: null
                                                                                                                                     , components: null)
                                                                               .Wait()));
        }
    }
}
