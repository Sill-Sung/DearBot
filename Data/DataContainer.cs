using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Discord.WebSocket;

namespace DearBot.Data
{
    internal class DataContainer
    {
        Dictionary<string, string>? dic_socketData = null;
        DiscordShardedClient? bot = null;

        SocketUser? user = null;
        SocketGuild? guild = null;
        SocketChannel? channel = null;

        string key = string.Empty;

        public SocketGuild Guild
        {
            get { return guild; }
        }
        public SocketChannel Channel
        {
            get { return channel; }
        }

        public SocketUser User
        {
            get { return user; }
        }

        public string Key
        {
            get { return key; }
        }

        public DataContainer(DiscordShardedClient bot, SocketMessageComponent socketMessageComponent)
        {
            dic_socketData = socketMessageComponent.Data.CustomId.Split(Convert.ToChar(06), StringSplitOptions.RemoveEmptyEntries).Select(x => x.Split("=")).ToDictionary(x => x[0], x => x[1]);

            this.user = socketMessageComponent.User;
            this.bot = bot;

            string str_guildID = string.Empty;
            string str_channelID = string.Empty;

            dic_socketData.TryGetValue("GUILD", out str_guildID);
            dic_socketData.TryGetValue("CHANNEL", out str_channelID);
            dic_socketData.TryGetValue("KEY", out this.key);

            if (string.IsNullOrEmpty(str_channelID) == false)
            {
                this.channel = bot.GetChannel(Convert.ToUInt64(str_channelID));
            }

            if (string.IsNullOrEmpty(str_guildID) == false)
            {
                this.guild = bot.GetGuild(Convert.ToUInt64(str_guildID));
            }
        }

        public DataContainer(DiscordShardedClient bot, SocketModal socketModal)
        {
            dic_socketData = socketModal.Data.CustomId.Split(Convert.ToChar(06), StringSplitOptions.RemoveEmptyEntries).Select(x => x.Split("=")).ToDictionary(x => x[0], x => x[1]);

            this.user = socketModal.User;
            this.bot = bot;

            string str_guildID = string.Empty;
            string str_channelID = string.Empty;

            dic_socketData.TryGetValue("GUILD", out str_guildID);
            dic_socketData.TryGetValue("CHANNEL", out str_channelID);
            dic_socketData.TryGetValue("KEY", out this.key);

            if (string.IsNullOrEmpty(str_channelID) == false)
            {
                this.channel = bot.GetChannel(Convert.ToUInt64(str_channelID));
            }

            if (string.IsNullOrEmpty(str_guildID) == false)
            {
                this.guild = bot.GetGuild(Convert.ToUInt64(str_guildID));
            }
        }
    }
}
