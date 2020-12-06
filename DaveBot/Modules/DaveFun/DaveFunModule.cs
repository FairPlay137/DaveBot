using System.Threading.Tasks;
using Discord.Commands;
using DaveBot.Common;

namespace DaveBot.Modules
{
    [Name("DaveBot Fun")]
    public class DaveFunModule : DaveBotTopModuleBase
    {
        [Command("fire")]
        [Summary("(To be written)")]
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
        [Summary("(To be written)")]
        public async Task CrotchKick([Remainder]string target)
        {
            await ReplyAsync(StringResourceHandler.GetTextStatic("DaveFun", "crotchkick", target));
            if (target.Contains(Context.Client.CurrentUser.Mention))
            {
                await ReplyAsync(StringResourceHandler.GetTextStatic("DaveFun", "crotchkick_kickedSelf"));
            }
        }

        [Command("clean")]
        [Summary("(To be written)")]
        public async Task GodClean([Remainder]string target)
        {
            await ReplyAsync(StringResourceHandler.GetTextStatic("DaveFun", "clean", target));
            if (target.Contains(Context.Client.CurrentUser.Mention))
            {
                await ReplyAsync(StringResourceHandler.GetTextStatic("DaveFun", "clean_easterEgg1"));
            }
        }

        [Command("shinkick")]
        [Summary("(To be written)")]
        public async Task ShinKick([Remainder] string target)
        {
            await ReplyAsync(StringResourceHandler.GetTextStatic("DaveFun", "shinkick", target));
        }
    }
}