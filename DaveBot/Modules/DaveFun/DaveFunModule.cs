using System.Threading.Tasks;
using Discord.Commands;
using DaveBot.Common;

namespace DaveBot.Modules
{
    [Name("Davemadson Fun")]
    public class DaveFunModule : DaveBotTopModuleBase
    {
        [Command("fire")]
        [Summary("Fires someone.")]
        public async Task Fire([Remainder]string target)
        {
            if(target == Context.User.Mention)
            {
                await ReplyAsync(StringResourceHandler.GetTextStatic("DaveFun", "fire_cannotFireYourself"));
                return;
            }
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
            if (target.ToUpperInvariant().Contains(Context.Client.CurrentUser.Mention))
            {
                await ReplyAsync(StringResourceHandler.GetTextStatic("DaveFun", "fire_easteregg3"));
                target = Context.User.Mention;
            }
            await ReplyAsync(StringResourceHandler.GetTextStatic("DaveFun", "fire", target.ToUpperInvariant()));
        }

        [Command("crotchkick")]
        [Summary("Crotch-kicks someone.")]
        public async Task CrotchKick([Remainder]string target)
        {
            await ReplyAsync(StringResourceHandler.GetTextStatic("DaveFun", "crotchkick", target));
            if (target.Contains(Context.Client.CurrentUser.Mention))
            {
                await ReplyAsync(StringResourceHandler.GetTextStatic("DaveFun", "crotchkick_kickedSelf"));
            }
        }

        [Command("clean")]
        [Summary("Sends someone to God's Country to get cleaned out.")]
        public async Task GodClean([Remainder]string target)
        {
            await ReplyAsync(StringResourceHandler.GetTextStatic("DaveFun", "clean", target));
            if (target.Contains(Context.Client.CurrentUser.Mention))
            {
                await ReplyAsync(StringResourceHandler.GetTextStatic("DaveFun", "clean_easterEgg1"));
            }
        }

        [Command("shinkick")]
        [Summary("Shin-kicks someone.")]
        public async Task ShinKick([Remainder] string target)
        {
            await ReplyAsync(StringResourceHandler.GetTextStatic("DaveFun", "shinkick", target));
        }

        [Command("wedgie")]
        [Summary("Gives someone a wedgie.")]
        public async Task Wedgie([Remainder] string target)
        {
            await ReplyAsync(StringResourceHandler.GetTextStatic("DaveFun", "wedgie", target));
        }
    }
}