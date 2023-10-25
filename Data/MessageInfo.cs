using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DearBot.Data
{
    public class MessageInfo
    {
        public MessageInfo(ulong messageID, ulong guildID, ulong userID, ulong? prevMessageID, ulong rootMessageID)
        {
            MessageID = messageID;
            GuildID = guildID;
            UserID = userID;
            PrevMessageID = prevMessageID;
            RootMessageID = rootMessageID;
        }

        public ulong MessageID { get; }
        public ulong GuildID { get; }
        public ulong UserID { get; }
        public ulong? PrevMessageID { get; }
        public ulong RootMessageID { get; }
    }
}