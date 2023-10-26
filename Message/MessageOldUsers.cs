using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;

using Discord;
using Discord.Rest;
using Discord.WebSocket;
using Discord.Interactions;

using GroupsV2.Utils;
using GroupsV2.Data.Response;
using GroupsV2.Data.Containers;

using Destiny.Utils;

namespace DearBot.Message
{
    internal class MessageOldUsers
    {
        private readonly ShardedInteractionContext _context;
        private readonly HttpClient _webClient;
        private readonly string _xApiKey;

        public MessageOldUsers(ShardedInteractionContext context)
        {
            IConfiguration configuration = new ConfigurationBuilder().SetBasePath(Environment.CurrentDirectory).AddJsonFile("config.json").Build();
            _webClient = new HttpClient();
            _context = context;

            _xApiKey = configuration.GetSection("X-API-Key").Value;
            _webClient.DefaultRequestHeaders.Add("X-API-Key", _xApiKey);
        }

        public async Task GetOldUsers(string clan_name, int limit_days)
        {
            DateTime nowDate = DateTime.Now;
            
            if (string.IsNullOrEmpty(clan_name))
                return;

            RestApplication app_client = _context.Client.GetApplicationInfoAsync().Result;

            GroupClanUtil clanUtil = new GroupClanUtil(_webClient);
            DestinyUtil destinyUtil = new DestinyUtil(_webClient);

            List<Embed> embeds = new List<Embed>();

            EmbedBuilder embedHeader = new EmbedBuilder();
            EmbedBuilder embedMessage = new EmbedBuilder();
            EmbedBuilder embedFooter = new EmbedBuilder();

            GroupResponse group = clanUtil.GetClanInfo(clan_name);

            // Message Author ----------------------------------------------------------------
            EmbedAuthorBuilder authorBuilder = new EmbedAuthorBuilder();
            authorBuilder.Name = string.Format("{0}", app_client.Name);
            authorBuilder.IconUrl = app_client.IconUrl;

            // Message Title -----------------------------------------------------------------
            StringBuilder sb_embedTitle = new StringBuilder();
            sb_embedTitle.AppendFormat("반가워요, {0}님!", _context.User.Username);

            // Message Description -----------------------------------------------------------
            StringBuilder sb_embedDesc = new StringBuilder();
            sb_embedDesc.Append("저는 DearBot예요.").Append(Environment.NewLine);
            sb_embedDesc.AppendFormat($"[{group.Info.Name}] 클랜의 {limit_days}일 이상 미접속 유저를 보여드릴게요.").Append(Environment.NewLine).Append(Environment.NewLine);

            embedHeader.WithAuthor(authorBuilder)
                       .WithTitle(sb_embedTitle.ToString())
                       .WithDescription(sb_embedDesc.ToString());

            embeds.Add(embedHeader.Build());

            await _context.Interaction.RespondAsync(text: null
                                                       , isTTS: false
                                                       , embed: embedHeader.Build()
                                                       , options: null
                                                       , ephemeral: true
                                                       , allowedMentions: null);
            
            clanUtil.SetDestinyUserProfile(ref group);
            List<GroupUserInfoBase> guildUsers = clanUtil.GetLongTermUnplayedMembers(group, limit_days);
            if (guildUsers == null || guildUsers.Count == 0)
                return;

            // Message Fields ----------------------------------------------------------------
            for (int idx_user = 0; idx_user < guildUsers.Count; idx_user++)
            {
                GroupUserInfoBase guildUser = guildUsers[idx_user];
                DateTime lastDate = guildUser.DestinyProfile.Profiles.MaxBy(x => x.DateLastPlayed).DateLastPlayed.Date;
                double elapsedDays = (nowDate.Date - lastDate.Date).TotalDays;

                EmbedFieldBuilder fBuild_join = new EmbedFieldBuilder();
                fBuild_join.WithName($"[{idx_user + 1}] {guildUser.DestinyUserInfo.BungieGlobalDisplayName}")
                           .WithValue($"{lastDate} [{elapsedDays}일]")
                           .WithIsInline(false);

                if (idx_user != 0 && (idx_user % EmbedBuilder.MaxFieldCount == 0 || idx_user == guildUsers.Count - 1))
                {
                    embeds.Add(embedMessage.Build());
                    await _context.Interaction.ModifyOriginalResponseAsync(x => x.Embeds = embeds.ToArray());

                    embedMessage = new EmbedBuilder();
                }

                embedMessage.AddField(fBuild_join);
            }

            EmbedFooterBuilder footerBuilder = new EmbedFooterBuilder();
            footerBuilder.WithText("Created at : " + _context.Interaction.CreatedAt.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss") + " Created by DearBot. Made by Dearest");
            footerBuilder.WithIconUrl(app_client.IconUrl);

            embedFooter.WithFooter(footerBuilder);

            embeds.Add(embedFooter.Build());

            await _context.Interaction.ModifyOriginalResponseAsync(x=> x.Embeds = embeds.ToArray());
        }

        public async Task GetOldUsersExcept(string clan_name, int limit_days, string except_role)
        {
            List<SocketRole> roles = _context.Guild.Roles.Where(x => x.Name.Equals(except_role, StringComparison.OrdinalIgnoreCase)).ToList();

            DateTime nowDate = DateTime.Now;

            if (string.IsNullOrEmpty(clan_name))
                return;

            RestApplication app_client = _context.Client.GetApplicationInfoAsync().Result;

            GroupClanUtil clanUtil = new GroupClanUtil(_webClient);
            DestinyUtil destinyUtil = new DestinyUtil(_webClient);

            List<Embed> embeds = new List<Embed>();

            EmbedBuilder embedHeader = new EmbedBuilder();
            EmbedBuilder embedMessage = new EmbedBuilder();
            EmbedBuilder embedFooter = new EmbedBuilder();

            GroupResponse group = clanUtil.GetClanInfo(clan_name);

            // Message Author ----------------------------------------------------------------
            EmbedAuthorBuilder authorBuilder = new EmbedAuthorBuilder();
            authorBuilder.Name = string.Format("{0}", app_client.Name);
            authorBuilder.IconUrl = app_client.IconUrl;

            // Message Title -----------------------------------------------------------------
            StringBuilder sb_embedTitle = new StringBuilder();
            sb_embedTitle.AppendFormat("반가워요, {0}님!", _context.User.Username);

            // Message Description -----------------------------------------------------------
            StringBuilder sb_embedDesc = new StringBuilder();
            sb_embedDesc.Append("저는 DearBot예요.").Append(Environment.NewLine);
            sb_embedDesc.AppendFormat($"[{group.Info.Name}] 클랜의 {limit_days}일 이상 미접속 유저를 보여드릴게요.").Append(Environment.NewLine).Append(Environment.NewLine);

            embedHeader.WithAuthor(authorBuilder)
                       .WithTitle(sb_embedTitle.ToString())
                       .WithDescription(sb_embedDesc.ToString());

            embeds.Add(embedHeader.Build());

            await _context.Interaction.RespondAsync(text: null
                                                       , isTTS: false
                                                       , embed: embedHeader.Build()
                                                       , options: null
                                                       , ephemeral: true
                                                       , allowedMentions: null);


            clanUtil.SetDestinyUserProfile(ref group);
            List<GroupUserInfoBase> guildUsers = clanUtil.GetLongTermUnplayedMembers(group, limit_days);
            if (guildUsers == null || guildUsers.Count == 0)
                return;

            // Message Fields ----------------------------------------------------------------
            int count = 0;

            foreach (GroupUserInfoBase guildUser in guildUsers)
            {
                List<SocketGuildUser> discordUsers = GetDiscordUser(guildUser.DestinyUserInfo.BungieGlobalDisplayName);

                if (discordUsers.Count > 0 && discordUsers.Where(x => x.Roles.Where(r => roles.Contains(r)).ToList().Count() > 0).Count() > 0)
                    continue;

                string name = discordUsers.Count == 0 ? $"[{count + 1}] {guildUser.DestinyUserInfo.BungieGlobalDisplayName} (Not Found Discord Profile)"
                                                      : $"[{count + 1}] {guildUser.DestinyUserInfo.BungieGlobalDisplayName} ({discordUsers[0].Mention})";

                DateTime lastDate = guildUser.DestinyProfile.Profiles.MaxBy(x => x.DateLastPlayed).DateLastPlayed.Date;
                double elapsedDays = (nowDate.Date - lastDate.Date).TotalDays;

                EmbedFieldBuilder fBuild_join = new EmbedFieldBuilder();
                fBuild_join.WithName($"[{count + 1}] {guildUser.DestinyUserInfo.BungieGlobalDisplayName}")
                           .WithValue($"{lastDate} [{elapsedDays}일]")
                           .WithIsInline(false);

                if (count != 0 && count % EmbedBuilder.MaxFieldCount == 0)
                {
                    embeds.Add(embedMessage.Build());
                    await _context.Interaction.ModifyOriginalResponseAsync(x => x.Embeds = embeds.ToArray());

                    embedMessage = new EmbedBuilder();
                }

                embedMessage.AddField(fBuild_join);
                count++;
            }

            if (embedMessage.Fields.Count > 0)
            {
                embeds.Add(embedMessage.Build());
                await _context.Interaction.ModifyOriginalResponseAsync(x => x.Embeds = embeds.ToArray());
            }

            EmbedFooterBuilder footerBuilder = new EmbedFooterBuilder();
            footerBuilder.WithText("Created at : " + _context.Interaction.CreatedAt.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss") + " Created by DearBot. Made by Dearest");
            footerBuilder.WithIconUrl(app_client.IconUrl);

            embedFooter.WithFooter(footerBuilder);

            embeds.Add(embedFooter.Build());

            await _context.Interaction.ModifyOriginalResponseAsync(x => x.Embeds = embeds.ToArray());
        }

        public List<SocketGuildUser> GetDiscordUser(string inGameName)
        {
            List<SocketGuildUser> users = new List<SocketGuildUser>();

            users = _context.Guild.Users.Where(x =>
            {
                string discord_name = x.DisplayName.ToLower().Replace(" ", "").Replace("_", "");
                string compare_name = inGameName.ToLower().Replace(" ", "").Replace("_", "");

                return discord_name.Contains(compare_name);
            }).ToList();

            return users;
        }
    }
}
