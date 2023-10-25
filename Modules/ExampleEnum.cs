using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Discord.Interactions;

namespace DearBot.Modules
{
    public enum ExampleEnum
    {
        First,
        Second,
        Third,
        Fourth,
        [ChoiceDisplay("Twenty First")]
        TwentyFirst
    }
}
