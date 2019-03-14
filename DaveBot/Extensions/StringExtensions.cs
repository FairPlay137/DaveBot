using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DaveBot.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Sanitizes @everyone and @here from a string.
        /// </summary>
        /// <param name="str">Original string</param>
        /// <returns>String with @everyone and @here sanitized.</returns>
        public static string SanitizeMentions(this string str) =>
            str.Replace("@everyone", "@everyοne").Replace("@here", "@һere"); //code from NadekoBot

        private static readonly Regex filterRegex = new Regex(@"(?:discord(?:\.gg|.me|app\.com\/invite)\/(?<id>([\w]{16}|(?:[\w]+-?){3})))", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        public static bool IsDiscordInvite(this string str)
            => filterRegex.IsMatch(str);

        public static string Unmention(this string str) => str.Replace("@", "ම");
    }
}
