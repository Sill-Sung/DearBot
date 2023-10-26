using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Discord;
using Discord.Utils;
using Discord.WebSocket;
using Discord.Interactions;

using DearBot.Data;
using DearBot.Utils;

namespace DearBot.Modal
{
    public class InterviewModal : IModal
    {
        public string Title => "면접 일자";

        [RequiredInput(true)]
        [InputLabel("면접 가능 일자")]        
        [ModalTextInput(customId:"day", style: TextInputStyle.Short, placeholder: "오늘 오후 8시 / 아무때나 / ...", maxLength: 100)]
        public string Day { get; set; }

        [RequiredInput(false)]
        [InputLabel("비고")]
        [ModalTextInput("comment", style: TextInputStyle.Paragraph, placeholder: "비고", maxLength: 500)]
        public string Comment { get; set; }

        public async Task ResponseInterview(ShardedInteractionContext context, MessageInfo messageInfo)
        {
            EmbedBuilder embed = new EmbedBuilder();
            SocketGuild guild = context.Client.GetGuild(messageInfo.GuildID);

            /* Message Author ----------------------------------------------------------------*/
            EmbedAuthorBuilder authorBuilder = new EmbedAuthorBuilder();
            authorBuilder.Name = string.Format("{0}", guild.Name);
            authorBuilder.IconUrl = guild.IconUrl != null ? guild.IconUrl : string.Empty;

            /* Message Title -----------------------------------------------------------------*/
            StringBuilder sb_embedTitle = new StringBuilder();
            sb_embedTitle.Append("클랜 가입 면접 신청이 완료되었습니다.");
            
            /* Message Description -----------------------------------------------------------*/
            StringBuilder sb_embedDesc = new StringBuilder();
            sb_embedDesc.Append($"정상적으로 {context.User.Mention}({MarkDown.EscapeMarkDown(context.User.Username)})님이 입력하신 내용을 관리자 분들께 전달했어요!").Append(Environment.NewLine);
            sb_embedDesc.Append($"별도의 문의 사항이나, 변경 사항이 있다면 언제든지 클랜 관리자분들께 DM 보내시면 됩니다!").Append(Environment.NewLine).Append(Environment.NewLine);
            sb_embedDesc.Append("< 관리자 목록 >").Append(Environment.NewLine);

            Dictionary<string, string> dic_adminUsers = new Dictionary<string, string>();
            List<SocketRole> adminRoles = guild.Roles.Where(x => x.Permissions.Administrator).ToList();

            dic_adminUsers.Add(guild.Owner.Mention, guild.Owner.Username);
            foreach (SocketRole role in adminRoles)
            {
                foreach (SocketUser user in role.Members)
                {
                    if (!dic_adminUsers.ContainsKey(user.Mention) && user.IsBot == false)
                        dic_adminUsers.Add(user.Mention, user.Username);
                }
            }

            dic_adminUsers.ToList().ToList().ForEach(x => sb_embedDesc.Append($"  {x.Key}({MarkDown.EscapeMarkDown(x.Value)})").Append(Environment.NewLine));

            /* Message Fields ----------------------------------------------------------------*/
            EmbedFieldBuilder fBuild_day = new EmbedFieldBuilder();
            fBuild_day.WithName("[면접 가능 시간]")
                       .WithValue(Day)
                       .WithIsInline(false);

            EmbedFieldBuilder fBuild_comment = new EmbedFieldBuilder();
            fBuild_comment.WithName("[면접 참고 사항]")
                       .WithValue(Comment)
                       .WithIsInline(false);

            /* Message Footer ----------------------------------------------------------------*/
            EmbedFooterBuilder footerBuilder = new EmbedFooterBuilder();
            footerBuilder.WithIconUrl(guild.IconUrl)
                         .WithText("Created at : " + context.Interaction.CreatedAt.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss") + " Created by DearBot. Made by Dearest");

            /* Set EmbedBuilder --------------------------------------------------------------*/
            embed.WithAuthor(authorBuilder)
                    .WithTitle(sb_embedTitle.ToString())
                    .WithDescription(sb_embedDesc.ToString())
                    .AddField(fBuild_day)
                    .AddField(fBuild_comment)
                    .WithFooter(footerBuilder);

            await Discord.UserExtensions.SendMessageAsync(user: context.User
                                                     , text: string.Empty
                                                     , isTTS: false
                                                     , embed: embed.Build()
                                                     , options: null
                                                     , allowedMentions: null
                                                     , components: null);
        }

        public async Task AnnounceToGuildAdmin(ShardedInteractionContext context, MessageInfo messageInfo)
        {
            SocketGuild guild = context.Client.GetGuild(messageInfo.GuildID);
            EmbedBuilder embed = new EmbedBuilder();

            // Message Author ----------------------------------------------------------------
            EmbedAuthorBuilder authorBuilder = new EmbedAuthorBuilder();
            authorBuilder.Name = string.Format("{0}", guild.Name);
            authorBuilder.IconUrl = guild.IconUrl != null ? guild.IconUrl : string.Empty;

            // Message Title -----------------------------------------------------------------
            StringBuilder sb_embedTitle = new StringBuilder();
            sb_embedTitle.Append("클랜 가입 신청 요청이 있습니다.");

            // Message Description -----------------------------------------------------------
            StringBuilder sb_embedDesc = new StringBuilder();
            sb_embedDesc.Append($"{context.User.Mention}({MarkDown.EscapeMarkDown(context.User.Username)})님이 클랜 가입 신청하셨습니다.");

            // Message Fields ----------------------------------------------------------------
            EmbedFieldBuilder fBuild_day = new EmbedFieldBuilder();
            fBuild_day.WithName("[면접 가능 시간]")
                       .WithValue(Day)
                       .WithIsInline(false);

            EmbedFieldBuilder fBuild_comment = new EmbedFieldBuilder();
            fBuild_comment.WithName("[면접 참고 사항]")
                       .WithValue(Comment)
                       .WithIsInline(false);

            // Message Footer ----------------------------------------------------------------
            EmbedFooterBuilder footerBuilder = new EmbedFooterBuilder();
            footerBuilder.WithIconUrl(guild.IconUrl)
                         .WithText("Created at : " + context.Interaction.CreatedAt.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss") + " Created by DearBot. Made by Dearest");

            // Set EmbedBuilder --------------------------------------------------------------
            embed.WithAuthor(authorBuilder)
                    .WithTitle(sb_embedTitle.ToString())
                    .WithDescription(sb_embedDesc.ToString())
                    .AddField(fBuild_day)
                    .AddField(fBuild_comment)
                    .WithFooter(footerBuilder);

            // Send message to Only Guild's [ Owner & Administrator ] ---------------------------------------------------------------------------------------------------------------
            await guild.DownloadUsersAsync();

            Dictionary<string, string> dic_adminUsers = new Dictionary<string, string>();
            List<SocketRole> adminRoles = guild.Roles.Where(x => x.Permissions.Administrator).ToList();

            dic_adminUsers.Add(guild.Owner.Mention, guild.Owner.Username);
            Discord.UserExtensions.SendMessageAsync(user: guild.Owner
                                                                , text: string.Empty
                                                                , isTTS: false
                                                                , embed: embed.Build()
                                                                , options: null
                                                                , allowedMentions: null
                                                                , components: null).Wait();

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
