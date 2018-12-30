using System;
using System.Resources;
using DaveBot.Resources;
using NLog;

/************************************************************/
/*                 String fetching routines                 */
/************************************************************/

namespace DaveBot.Common
{
    class StringResourceHandler
    {
        public static ResourceManager StringsRsManager { get; } = new ResourceManager(typeof(BotStrings));

        public static string GetTextStatic(string category, string key)
        {
            var text = StringsRsManager.GetString(category + "_" + key);

            if (string.IsNullOrWhiteSpace(text))
            {
                LogManager.GetCurrentClassLogger().Warn(category + "_" + key + " key is missing from BotStrings! Report this ASAP.");
                text = $"!! Key \"{category + "_" + key}\" not found in BotStrings !!";
            }
            return text;
        }
        public static string GetTextStatic(string category, string key,
            params object[] replacements)
        {
            try
            {
                return string.Format(GetTextStatic(category, key), replacements);
            }
            catch (FormatException)
            {
                LogManager.GetCurrentClassLogger().Warn(category + "_" + key + " key is not formatted correctly! Report this ASAP.");
                return $"Error: Key {category + "_" + key} is not formatted correctly!";
            }
        }

        public static bool DoesTextExistStatic(string category, string key)
        {
            var text = StringsRsManager.GetString(category + "_" + key); //retrieve the string in "category_key" format
            return string.IsNullOrWhiteSpace(text); //return whether the string exists or not
        }

        public string GetText(string category, string key)
            => GetTextStatic(category, key);
        public string GetText(string category, string key,
            params object[] replacements)
            => GetTextStatic(category, key, replacements);
        public bool DoesTextExist(string category, string key)
            => DoesTextExistStatic(category, key);
    }
}
