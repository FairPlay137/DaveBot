using System.Threading.Tasks;
using Discord.Commands;
using DaveBot.Common;

namespace DaveBot.Modules
{
    [Name("DaveBot Fun")]
    public class DaveFunModule : DaveBotTopModuleBase
    {
        [Command("fire")]
        [Summary("Placeholder summary")]
        public async Task Fire([Remainder]string target)
        {
            if(target.ToUpperInvariant() == StringResourceHandler.GetTextStatic("DaveFun", "fire_egg1trigger"))
            {
                await ReplyAsync(StringResourceHandler.GetTextStatic("DaveFun", "fire_easteregg1"));
                return;
            }
            if (target.ToUpperInvariant() == StringResourceHandler.GetTextStatic("DaveFun", "fire_egg2trigger"))
            {
                await ReplyAsync(StringResourceHandler.GetTextStatic("DaveFun", "fire_easteregg2"));
                return;
            }
            await ReplyAsync(StringResourceHandler.GetTextStatic("DaveFun", "fire", target.ToUpperInvariant()));
        }
    }
}