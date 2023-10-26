using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DearBot.Utils
{
    internal class MarkDown
    {
        public static string EscapeMarkDown(string message)
        {
            return message.Replace("*", "\\*")
                          .Replace("_", "\\_")
                          .Replace("~", "\\~");
        }
    }
}
