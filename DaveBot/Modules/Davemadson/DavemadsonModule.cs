using Discord.Commands;
using DaveBot.Common;
using System.Threading.Tasks;

namespace DaveBot.Modules
{
    [Name("Davemadson")]
    public class DavemadsonModule : DaveBotModuleBase<SocketCommandContext>
    {
        [Command("davemadson")]
        [Summary("Command description TBD")]
        public async Task DavemadsonCmd()
        {
            await ReplyAsync($"{StringResourceHandler.GetTextStatic("Davemadson", "davemadson")} https://www.youtube.com/channel/UCGBiNqgNTsNUgRkUvAr4jWA");
        }

        [Command("ltbloopers")]
        [Summary("Command description TBD")]
        public async Task LTBloopersCmd()
        {
            await ReplyAsync($"{StringResourceHandler.GetTextStatic("Davemadson", "ltbloopers")} https://www.youtube.com/playlist?list=PLgN-B1Cy8AZ5bHrcnGkXz4Yh9CkLuPwnC");
        }

        [Command("funnysigns")]
        [Summary("Command description TBD")]
        public async Task FunnySignsCmd()
        {
            await ReplyAsync($"{StringResourceHandler.GetTextStatic("Davemadson", "funnysigns")} https://www.youtube.com/playlist?list=PL6Wcr6NPTjmEJQpM4qhIIQGxsL2pyq_BF");
        }
    }
}