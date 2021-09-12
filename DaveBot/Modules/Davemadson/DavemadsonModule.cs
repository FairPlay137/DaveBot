using Discord.Commands;
using DaveBot.Common;
using System.Threading.Tasks;
using Discord;

namespace DaveBot.Modules
{
    [Name("Davemadson")]
    public class DavemadsonModule : DaveBotTopModuleBase
    {
        [Command("davemadson")]
        [Summary("Posts a link to davemadson's YouTube channel")]
        public async Task DavemadsonCmd()
        {
            await Context.Message.ReplyAsync($"{StringResourceHandler.GetTextStatic("Davemadson", "davemadson")} https://www.youtube.com/channel/UCGBiNqgNTsNUgRkUvAr4jWA");
        }

        [Command("ltbloopers")]
        [Summary("Posts a link to File Preserver's LT Bloopers playlist")]
        [Alias("bloopers")]
        public async Task LTBloopersCmd()
        {
            await Context.Message.ReplyAsync($"{StringResourceHandler.GetTextStatic("Davemadson", "ltbloopers")} https://www.youtube.com/playlist?list=PLgN-B1Cy8AZ5bHrcnGkXz4Yh9CkLuPwnC");
        }

        [Command("funnysigns")]
        [Summary("Posts a link to a list of Angus McTavish's playlist for davemadson's Funny Signs (from April 2014 and before)")]
        public async Task FunnySignsCmd()
        {
            await Context.Message.ReplyAsync($"{StringResourceHandler.GetTextStatic("Davemadson", "funnysigns")} https://www.youtube.com/playlist?list=PL6Wcr6NPTjmEJQpM4qhIIQGxsL2pyq_BF");
        }
    }
}