using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Commands.Builders;
using System.Diagnostics;
using System.Reflection;
using NLog;
using DaveBot.Common;
using DaveBot.Common.Attributes;
using DaveBot.Services;

namespace DaveBot.Modules
{
    public class AdminModule : DaveBotModuleBase<CommandContext>
    {
        private readonly IBotConfiguration _config;

        public Admin(IBotConfiguration config)
        {
            _config = config;
        }

        [Command("shutdown")]
        [Summary("Shuts down the bot. **BOT OWNER ONLY**")]
        [Alias("die")]
        [OwnerOnly]
        public async Task Shutdown()
        {
            await ReplyAsync($":ok: `{StringResourceHandler.GetTextStatic("Admin", "shutdown")}`").ConfigureAwait(false);
            LogManager.GetCurrentClassLogger().Info(">>SHUTTING DOWN");
            _config.SaveConfig(true);
            LogManager.GetCurrentClassLogger().Info("Logging out...");
            await Context.Client.LogoutAsync().ConfigureAwait(false);
            LogManager.GetCurrentClassLogger().Info("The bot is now DOWN!");
            await Task.Delay(2000).ConfigureAwait(false);
            Environment.Exit(0);
        }
        [Command("restart")]
        [Summary("Restarts the bot. **BOT OWNER ONLY**")]
        [OwnerOnly]
        public async Task Restart()
        {
            await ReplyAsync($":ok: `{StringResourceHandler.GetTextStatic("Admin", "restart")}`");
            LogManager.GetCurrentClassLogger().Info(">>RESTARTING");
            _config.SaveConfig(true);
            LogManager.GetCurrentClassLogger().Info("Logging out...");
            await Context.Client.LogoutAsync().ConfigureAwait(false);
            LogManager.GetCurrentClassLogger().Info("Relaunching in 2 seconds...");
            await Task.Delay(2000).ConfigureAwait(false);
            Process.Start(Assembly.GetExecutingAssembly().Location);
            Environment.Exit(0);
        }
        [Command("setgame")]
        [Summary("Sets the bot's game. **BOT OWNER ONLY**")]
        [OwnerOnly]
        public async Task SetGame(string atype, [Remainder] [Summary("Game to show on status")] string game)
        {
            ActivityType acttype;
            switch(atype.ToLowerInvariant())
            {
                case "playing":
                    acttype = ActivityType.Playing;
                    break;
                case "watching":
                    acttype = ActivityType.Watching;
                    break;
                case "listeningto":
                    acttype = ActivityType.Listening;
                    break;
                default:
                    throw new CommandUnsuccessfulException(StringResourceHandler.GetTextStatic("err", "invalidActivityType"));
            }
            await Context.Client.SetGameAsync(game, null, acttype).ConfigureAwait(false);
            await ReplyAsync($":ok: `{StringResourceHandler.GetTextStatic("Admin", "setGame",game)}`").ConfigureAwait(false);
        }
        [Command("setstatus")]
        [Summary("Sets the bot's status. **BOT OWNER ONLY**")]
        [OwnerOnly]
        public async Task SetStatus([Summary("Status (Online/Idle/DnD/Invisible)")] string status)
        {
            switch (status.ToLowerInvariant())
            {
                case "online":
                    await Context.Client.SetStatusAsync(UserStatus.Online);
                    break;
                case "idle":
                    await Context.Client.SetStatusAsync(UserStatus.Idle);
                    break;
                case "dnd":
                    await Context.Client.SetStatusAsync(UserStatus.DoNotDisturb);
                    break;
                case "invisible":
                    await Context.Client.SetStatusAsync(UserStatus.Invisible);
                    break;
                default:
                    throw new CommandUnsuccessfulException(StringResourceHandler.GetTextStatic("err", "invalidStatus"));
            }
            await ReplyAsync($":ok: `{StringResourceHandler.GetTextStatic("Admin", "setStatus")}`");
        }
        [Command("verboseerrors")]
        [Summary("Enables/disables verbose error messages **BOT OWNER ONLY**")]
        [Alias("ve")]
        [OwnerOnly]
        public async Task ToggleVerboseErrors()
        {
            _config.VerboseErrors = !_config.VerboseErrors;
            _config.SaveConfig(true);
            _config.ReloadConfig(false);
            string toCueUp = "verboseErrors_disable";
            if(_config.VerboseErrors)
                toCueUp = "verboseErrors_enable";
            await ReplyAsync($":ok: `{StringResourceHandler.GetTextStatic("Admin", toCueUp)}`").ConfigureAwait(false);
        }
    }
}
