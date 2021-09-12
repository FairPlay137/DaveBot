using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaveBot.Common.Attributes
{
    // An extension to allow for dedicated slash commands
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class SlashCommandAttribute : Attribute
    {
        public SlashCommandAttribute()
        {
            Text = null;
        }
        public SlashCommandAttribute(string text)
        {
            Text = text;
        }
        public SlashCommandAttribute(string text, bool ignoreExtraArgs)
        {
            Text = text;
            IgnoreExtraArgs = ignoreExtraArgs;
        }

        public string Text { get; }
        public RunMode RunMode { get; set; }
        public bool? IgnoreExtraArgs { get; }
    }
}
