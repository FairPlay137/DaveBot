﻿#if !PUBLIC_BUILD && !RELEASE
using System.Threading.Tasks;
using Discord.Commands;
using DaveBot.Common;
using DaveBot.Modules.Akinator.Services;

// This module is super-early in development, and as such is only available in debug builds of DaveBot.

namespace DaveBot.Modules
{
    [Name("Akinator")]
    public class AkinatorModule : DaveBotTopModuleBase<AkinatorService>
    {
        [Command("akinator")]
        [Summary("Starts an Akinator game (COMING SOON)")]
        [Alias("aki")]
        public async Task StartAkinator()
        {
            await ReplyAsync($"`{StringResourceHandler.GetTextStatic("Akinator", "comingSoon")}`").ConfigureAwait(false);
        }
        [Command("akistop")]
        [Summary("Stops the currently running Akinator game (COMING SOON)")]
        public async Task StopAkinator()
        {
            await ReplyAsync($"`{StringResourceHandler.GetTextStatic("Akinator", "comingSoon")}`").ConfigureAwait(false);
        }
        [Command("akianswer")]
        [Summary("Within Akinator: Answers a question (COMING SOON)")]
        [Alias("aa")]
        public async Task SendAnswer(string answer)
        {
            await ReplyAsync($"`{StringResourceHandler.GetTextStatic("Akinator", "comingSoon")}`").ConfigureAwait(false);
        }
    }
}
#endif