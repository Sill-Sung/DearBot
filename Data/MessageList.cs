using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DearBot.Data
{
    public class MessageList
    {
        private static List<MessageInfo> messages = new List<MessageInfo>();
        public static List<MessageInfo> Messages { get { return messages; } }

        public static ulong GetRootMessage(ulong messageId)
        {
            return messages.Find(x=> x.MessageID== messageId).RootMessageID;
        }

        public static ulong? GetPrevMessage(ulong messageId)
        {
            return messages.Find(x => x.MessageID == messageId).PrevMessageID;
        }

        public static MessageInfo GetMessageInfo(ulong messageId)
        {
            return messages.Find(x => x.MessageID == messageId);
        }

        public static void RemoveMessageInfos(ulong rootMessageId)
        {
            messages.Where(x => x.RootMessageID == rootMessageId).ToList().ForEach(x => messages.Remove(x));
        }
    }
}
