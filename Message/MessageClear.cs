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
    internal class MessageClear
    {
        private readonly DiscordShardedClient botClient;

        SocketMessage message = null;
        SocketGuild guild = null;
        SocketUser user;

        public MessageClear(DiscordShardedClient arg_bot, SocketMessage arg_message, SocketGuild arg_guild)
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

            ButtonBuilder btb_clanJoin = new ButtonBuilder(label: "네", customId: "clear_yes", style: ButtonStyle.Primary, url: null, emote: null, isDisabled: false);
            ButtonBuilder btb_customer = new ButtonBuilder(label: "아니요", customId: "clear_no", style: ButtonStyle.Secondary, url: null, emote: null, isDisabled: false);

            componentBuilder.WithButton(btb_clanJoin);
            componentBuilder.WithButton(btb_customer);

            await message.Channel.SendMessageAsync(text: null
                                                   , isTTS: false
                                                   , embed: embedBuilder.Build()
                                                   , options: null
                                                   , allowedMentions: null
                                                   , components: componentBuilder.Build());
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
            sb_embedDesc.AppendFormat("저는 {0}의 비서, DearBot예요.", guild.Name).Append(Environment.NewLine);
            sb_embedDesc.AppendFormat(@"저와의 모든 기록을 삭제하시겠어요?", user.Mention, user.Username).Append(Environment.NewLine);

            /* Message Fields ----------------------------------------------------------------*/
            EmbedFieldBuilder fBuild_join = new EmbedFieldBuilder();
            fBuild_join.WithName("[네]")
                       .WithValue("전송한 모든 메시지가 삭제됩니다.")
                       .WithIsInline(false);

            EmbedFieldBuilder fBuild_cust = new EmbedFieldBuilder();
            fBuild_cust.WithName("[아니요]")
                       .WithValue("현재 과정에서 빠져나옵니다.")
                       .WithIsInline(false);

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

        public async Task ClearMessage(SocketMessageComponent messageComponent)
        {
            List<IMessage> messages = await messageComponent.Channel.GetMessagesAsync().Flatten().ToListAsync();

            foreach (IMessage message in messages)
            {
                await message.DeleteAsync();
            }

            if ((await messageComponent.Channel.GetMessagesAsync().Flatten().AnyAsync()) == true)
                await ClearMessage(messageComponent);
        }

        public async Task ClearDMMessage(SocketMessageComponent messageComponent)
        {
            List<IMessage> messages = await messageComponent.Channel.GetMessagesAsync().Flatten().Where(x=> x.Author.IsBot).ToListAsync();

            foreach (IMessage message in messages)
            {
                await message.DeleteAsync();
            }

            if ((await messageComponent.Channel.GetMessagesAsync().Flatten().Where(x => x.Author.IsBot).AnyAsync()) == true)
                await ClearDMMessage(messageComponent);
        }
    }
}
