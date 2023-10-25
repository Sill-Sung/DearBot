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
    internal class MessageWelcome
    {
        private readonly DiscordShardedClient botClient;
        
        SocketMessage message = null;
        SocketGuild guild = null;
        SocketUser user;

        public MessageWelcome(DiscordShardedClient arg_bot, SocketMessage arg_message, SocketGuild arg_guild)
        {
            botClient = arg_bot;
            message = arg_message;
            guild = arg_guild;
            user = arg_message.Author;
        }

        public async Task SendMessage()
        {
            ComponentBuilder componentBuilder = new ComponentBuilder();
            
            EmbedBuilder embedBuilder = GetEmbedBuilder();
            
            ButtonBuilder btb_clanJoin = new ButtonBuilder(label: "가입 신청", customId: "join", style: ButtonStyle.Primary, url: null, emote: null, isDisabled: false);
            ButtonBuilder btb_customer = new ButtonBuilder(label: "손님", customId: "customer", style: ButtonStyle.Secondary, url: null, emote: null, isDisabled: false);
            
            componentBuilder.WithButton(btb_clanJoin);
            componentBuilder.WithButton(btb_customer);

            IUserMessage result = await Discord.UserExtensions.SendMessageAsync(user: user
                                                          , text: null
                                                          , isTTS: false
                                                          , embed: embedBuilder.Build()
                                                          , options: null
                                                          , allowedMentions: null
                                                          , components: componentBuilder.Build());
            
            MessageList.Messages.Add(new MessageInfo(message.Id, guild.Id, user.Id, null, message.Id));
            MessageList.Messages.Add(new MessageInfo(result.Id, guild.Id, user.Id, message.Id, message.Id));
        }

        private EmbedBuilder GetEmbedBuilder()
        {
            EmbedBuilder eBuilder = new Discord.EmbedBuilder();

            /* Message Author ----------------------------------------------------------------*/
            EmbedAuthorBuilder authorBuilder = new EmbedAuthorBuilder();
            authorBuilder.Name = string.Format("{0}", guild.Name);
            authorBuilder.IconUrl = guild.IconUrl != null ? guild.IconUrl : string.Empty;

            /* Message Title -----------------------------------------------------------------*/
            StringBuilder sb_embedTitle = new StringBuilder();
            sb_embedTitle.AppendFormat("반가워요, {0}님!", user.Username);

            /* Message Description -----------------------------------------------------------*/
            StringBuilder sb_embedDesc = new StringBuilder();
            sb_embedDesc.Append($"저는 {guild.Name}의 비서, DearBot예요.").Append(Environment.NewLine);
            sb_embedDesc.Append($@"저는 {user.Mention}[**{user.Username}**]님이 어떠한 사유로 입장하셨는지 궁금해요!").Append(Environment.NewLine);
            sb_embedDesc.Append($"{user.Mention}[**{user.Username}**]님을 반갑게 맞이할 수 있도록, 선택 부탁드려요!").Append(Environment.NewLine);

            /* Message Fields ----------------------------------------------------------------*/
            EmbedFieldBuilder fBuild_join = new EmbedFieldBuilder();
            fBuild_join.WithName("[가입 신청]")
                       .WithValue("클랜 가입을 위한 간단한 질문을 통해, 클랜 관리자에게 가입 신청 메시지가 자동 전송됩니다.")
                       .WithIsInline(true);

            EmbedFieldBuilder fBuild_cust = new EmbedFieldBuilder();
            fBuild_cust.WithName("[손님]")
                       .WithValue("일부 권한이 제한된 **손님** 권한이 자동 부여되며, 클랜 관리자에게 알림 메시지가 자동 전송됩니다.")
                       .WithIsInline(true);

            /* Message Footer ----------------------------------------------------------------*/
            EmbedFooterBuilder footerBuilder = new EmbedFooterBuilder();
            footerBuilder.WithIconUrl(guild.IconUrl)
                         .WithText("Created at : " + message.CreatedAt.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss") + " Created by DearBot. Made by Dearest");

            /* Set EmbedBuilder --------------------------------------------------------------*/
            eBuilder.WithAuthor(authorBuilder)
                    .WithTitle(sb_embedTitle.ToString())
                    .WithDescription(sb_embedDesc.ToString())
                    .AddField(fBuild_join)
                    .AddField(fBuild_cust)
                    .WithFooter(footerBuilder);

            return eBuilder;
        }

        private EmbedFieldBuilder CreateFieldBuilder(string arg_context, object arg_value, bool arg_IsInline)
        {
            EmbedFieldBuilder embedFieldBuilder = new EmbedFieldBuilder();

            embedFieldBuilder.Name = arg_context;
            embedFieldBuilder.IsInline = arg_IsInline;
            embedFieldBuilder.Value = arg_value == null ? "" : arg_value;

            return embedFieldBuilder;
        }

    }
}
