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

        private Task ModalSubmitted(SocketModal arg_socketModal)
        {
            botClient.ModalSubmitted -= ModalSubmitted;

            arg_socketModal.RespondAsync();             // Delete Previus Message (Select Purpose)
            SendMessageToAdministrator(arg_socketModal);
            arg_socketModal.Message.DeleteAsync();      // Download All Users in Guild

            List<SocketMessageComponentData> data = arg_socketModal.Data.Components.ToList();
            EmbedBuilder embed = new EmbedBuilder();

            /* Message Author ----------------------------------------------------------------*/
            EmbedAuthorBuilder authorBuilder = new EmbedAuthorBuilder();
            authorBuilder.Name = string.Format("{0}", dataContainer.Guild.Name);
            authorBuilder.IconUrl = dataContainer.Guild.IconUrl != null ? dataContainer.Guild.IconUrl : string.Empty;

            /* Message Title -----------------------------------------------------------------*/
            StringBuilder sb_embedTitle = new StringBuilder();
            sb_embedTitle.Append("클랜 가입 면접 신청이 완료되었습니다.");

            /* Message Description -----------------------------------------------------------*/
            StringBuilder sb_embedDesc = new StringBuilder();
            sb_embedDesc.Append($"정상적으로 {dataContainer.User.Mention}({dataContainer.User.Username})님이 입력하신 내용을 관리자 분들께 전달했어요!").Append(Environment.NewLine);
            sb_embedDesc.Append($"별도의 문의 사항이나, 변경 사항이 있다면 언제든지 클랜 관리자분들께 DM 보내시면 됩니다!").Append(Environment.NewLine).Append(Environment.NewLine);
            sb_embedDesc.Append("< 관리자 목록 >").Append(Environment.NewLine);

            Dictionary<string, string> dic_adminUsers = new Dictionary<string, string>();
            List<SocketRole> adminRoles = dataContainer.Guild.Roles.Where(x => x.Permissions.Administrator).ToList();

            dic_adminUsers.Add(dataContainer.Guild.Owner.Mention, dataContainer.Guild.Owner.Username);
            foreach (SocketRole role in adminRoles)
            {
                foreach(SocketUser user in role.Members)
                {
                    if (!dic_adminUsers.ContainsKey(user.Mention) && user.IsBot == false)
                        dic_adminUsers.Add(user.Mention, user.Username);
                }
            }

            dic_adminUsers.ToList().ToList().ForEach(x => sb_embedDesc.Append($"  {x.Key}({x.Value})").Append(Environment.NewLine));

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

            return Discord.UserExtensions.SendMessageAsync(user: arg_socketModal.User
                                                           , text: string.Empty
                                                           , isTTS: false
                                                           , embed: embed.Build());
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

        public void SendMessageToAdministrator(SocketModal arg_socketModal)
        {
            dataContainer.Guild.DownloadUsersAsync(); // Download All Users in Guild

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

                        Discord.UserExtensions.SendMessageAsync(user: user
                                                                , text: string.Empty
                                                                , isTTS: false
                                                                , embed: embed.Build()
                                                                , options: null
                                                                , allowedMentions: null
                                                                , components: null).Wait();
                    }
                }
            }
        }
    }
}
