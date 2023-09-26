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
        private readonly DiscordSocketClient botClient;
        
        SocketMessage message = null;
        SocketGuild guild = null;
        SocketUser user;

        public MessageWelcome(DiscordSocketClient arg_bot, SocketMessage arg_message, SocketGuild arg_guild)
        {
            botClient = arg_bot;
            message = arg_message;
            guild = arg_guild;
            user = arg_message.Author;
        }

        public Task SendMessage()
        {
            ComponentBuilder componentBuilder = new ComponentBuilder();

            EmbedBuilder embedBuilder = GetEmbedBuilder();
            ButtonBuilder btb_clanJoin = new ButtonBuilder("가입 신청", DataContainerUtil.CreateStringData(message, guild, "join"), ButtonStyle.Primary, null, null, false);
            ButtonBuilder btb_customer = new ButtonBuilder("손님", DataContainerUtil.CreateStringData(message, guild, "customer"), ButtonStyle.Secondary, null, null, false);

            componentBuilder.WithButton(btb_clanJoin);
            componentBuilder.WithButton(btb_customer);
            
            return Discord.UserExtensions.SendMessageAsync(user, null, false, embedBuilder.Build(), null, null, componentBuilder.Build());
        }

        private EmbedBuilder GetEmbedBuilder()
        {
            EmbedBuilder embedBuilder = new Discord.EmbedBuilder();
            EmbedAuthorBuilder embedAuthorBuilder = new EmbedAuthorBuilder();

            StringBuilder sb_embedTitle = new StringBuilder();
            StringBuilder sb_embedDesc = new StringBuilder();

            /* Message Title */
            sb_embedTitle.AppendFormat("[{0}] 서버 입장을 환영합니다!", guild.Name);

            /* Message Description */
            sb_embedDesc.AppendFormat("저희 [{0}] 클랜 서버는 {1} ({2})님이 어떠한 사유로 입장하셨는지 궁금해요!", guild.Name, user.Mention, user.Username);
            sb_embedDesc.Append(Environment.NewLine);
            sb_embedDesc.Append("원활한 유저 관리를 위해, 선택 부탁드립니다! :)");

            /* Message Author */
            if (guild.IconUrl != null)
            {
                embedAuthorBuilder.Name = string.Format("[{0}] 서버 입장을 환영합니다!", guild.Name);
                embedAuthorBuilder.IconUrl = guild.IconUrl;
            }

            /* Set EmbedBuilder ----------------------------------------------------*/
            
            embedBuilder.WithTitle(sb_embedTitle.ToString())
                    .WithDescription(sb_embedDesc.ToString())
                    .WithAuthor(new EmbedAuthorBuilder().WithUrl(guild.VanityURLCode)
                                                        .WithName(guild.Name)
                                                        .WithIconUrl(guild.IconUrl))
                    .WithFooter(new EmbedFooterBuilder().WithIconUrl(guild.IconUrl)
                                                        .WithText("원활한 유저 관리를 위해, 선택 부탁드립니다! :)"))
                    .AddField(new EmbedFieldBuilder().WithName(string.Format("저희 [{0}] 클랜 서버는 {1}님이 어떠한 사유로 입장하셨는지 궁금해요!", guild.Name, user.Mention))
                                                     .WithValue("원활한 유저 관리를 위해, 입장 목적을 알려주시면 감사하겠습니다! :)")
                                                     .WithIsInline(false));

            return embedBuilder;
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
