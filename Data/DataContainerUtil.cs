using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Discord;
using Discord.WebSocket;

namespace DearBot.Data
{
    internal static class DataContainerUtil
    {
        public static string CreateStringData(SocketMessage arg_msg, SocketGuild arg_guild, string arg_key)
        {
            StringBuilder sb_CustomData = new StringBuilder();

            sb_CustomData.AppendFormat("KEY={0}", arg_key).Append(Convert.ToChar(06));
            sb_CustomData.AppendFormat("GUILD={0}", arg_guild.Id).Append(Convert.ToChar(06));
            sb_CustomData.AppendFormat("USER={0}", arg_msg.Author.Id).Append(Convert.ToChar(06));
            sb_CustomData.AppendFormat("CHANNEL={0}", arg_msg.Channel.Id).Append(Convert.ToChar(06));

            return sb_CustomData.ToString(0, sb_CustomData.Length - 1);
        }

        public static string CreateStringData(SocketGuild arg_guild, SocketUser arg_user, ISocketMessageChannel arg_channel, string arg_key)
        {
            StringBuilder sb_CustomData = new StringBuilder();

            sb_CustomData.AppendFormat("KEY={0}", arg_key).Append(Convert.ToChar(06));
            sb_CustomData.AppendFormat("GUILD={0}", arg_guild.Id).Append(Convert.ToChar(06));
            sb_CustomData.AppendFormat("USER={0}", arg_user.Id).Append(Convert.ToChar(06));
            sb_CustomData.AppendFormat("CHANNEL={0}", arg_channel.Id).Append(Convert.ToChar(06));

            return sb_CustomData.ToString(0, sb_CustomData.Length - 1);
        }
    }
}
