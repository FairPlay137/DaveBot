using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using System.Diagnostics;
using System.Reflection;
using NLog;
using DaveBot.Common;
using DaveBot.Common.Attributes;
using DaveBot.Services;

namespace DaveBot.Modules
{
    [Name("Core")]
    public class CoreModule : DaveBotTopModuleBase
    {
        private readonly IBotConfiguration _config;

        public CoreModule(IBotConfiguration config)
        {
            _config = config;
        }

        [Command("shutdown")]
        [Summary("Shuts down the bot. **BOT OWNER ONLY**")]
        [Alias("die")]
        [OwnerOnly]
        public async Task Shutdown(bool SaveChanges = true)
        {
            //await Context.Message.AddReactionAsync(new Emoji("👋"));
            await Context.Message.ReplyAsync($":ok_hand: `{StringResourceHandler.GetTextStatic("Admin", "shutdown")}`").ConfigureAwait(false);
            LogManager.GetCurrentClassLogger().Info(">>SHUTTING DOWN");
            if(SaveChanges)
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
        public async Task Restart(bool SaveChanges = true)
        {
            //await Context.Message.AddReactionAsync(new Emoji("👋"));
            await Context.Message.ReplyAsync($":ok_hand: `{StringResourceHandler.GetTextStatic("Admin", "restart")}`");
            LogManager.GetCurrentClassLogger().Info(">>RESTARTING");
            if(SaveChanges)
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
            string acttypestring;
            switch(atype.ToLowerInvariant())
            {
                case "playing":
                    acttype = ActivityType.Playing;
                    acttypestring = StringResourceHandler.GetTextStatic("Admin", "setGame_playing");
                    break;
                case "watching":
                    acttype = ActivityType.Watching;
                    acttypestring = StringResourceHandler.GetTextStatic("Admin", "setGame_watching");
                    break;
                case "listeningto":
                    acttype = ActivityType.Listening;
                    acttypestring = StringResourceHandler.GetTextStatic("Admin", "setGame_listeningTo");
                    break;
                default:
                    throw new CommandUnsuccessfulException(StringResourceHandler.GetTextStatic("err", "invalidActivityType"));
            }
            await Context.Client.SetGameAsync(game, null, acttype).ConfigureAwait(false);
            //await Context.Message.AddReactionAsync(new Emoji("👌"));
            await Context.Message.ReplyAsync($":ok: `{StringResourceHandler.GetTextStatic("Admin", "setGame", game, acttypestring)}`").ConfigureAwait(false);
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
            //await Context.Message.AddReactionAsync(new Emoji("👌"));
            await Context.Message.ReplyAsync($":ok_hand: `{StringResourceHandler.GetTextStatic("Admin", "setStatus")}`");
        }
        [Command("verboseerrors")]
        [Summary("Enables/disables verbose error messages. **BOT OWNER ONLY**")]
        [Alias("ve")]
        [OwnerOnly]
        public async Task ToggleVerboseErrors()
        {
            //await Context.Message.AddReactionAsync(new Emoji("👌"));
            _config.VerboseErrors = !_config.VerboseErrors;
            _config.SaveConfig(true);
            _config.ReloadConfig(false);
            var toCueUp = "verboseErrors_disable";
            if(_config.VerboseErrors)
                toCueUp = "verboseErrors_enable";
            await Context.Message.ReplyAsync($":ok_hand: `{StringResourceHandler.GetTextStatic("Admin", toCueUp)}`").ConfigureAwait(false);
        }

        [Command("setshards")]
        [Summary("Sets the total shard count, then restarts the bot. **BOT OWNER ONLY**")]
        [OwnerOnly]
        public async Task SetTotalShardCount(int shards)
        {
            if(shards <= 0)
                throw new CommandUnsuccessfulException(StringResourceHandler.GetTextStatic("err", "invalidShardCount",shards));
            _config.TotalShards = shards;
            _config.SaveConfig(true);
            _config.ReloadConfig(false);
            //await Context.Message.AddReactionAsync(new Emoji("👌"));
            await Context.Message.ReplyAsync($":ok_hand: `{StringResourceHandler.GetTextStatic("Admin", "setShardCount", shards)}`").ConfigureAwait(false);
            LogManager.GetCurrentClassLogger().Info($"Total shard count set to {shards}. Now restarting bot...");
            LogManager.GetCurrentClassLogger().Info(">>RESTARTING");
            _config.SaveConfig(true);
            LogManager.GetCurrentClassLogger().Info("Logging out...");
            await Context.Client.LogoutAsync().ConfigureAwait(false);
            LogManager.GetCurrentClassLogger().Info("Relaunching in 2 seconds...");
            await Task.Delay(2000).ConfigureAwait(false);
            Process.Start(Assembly.GetExecutingAssembly().Location);
            Environment.Exit(0);
        }

        [Command("reloadconfig")]
        [Summary("Reloads the configuration from config.json. **BOT OWNER ONLY**")]
        [Alias("rcf")]
        [OwnerOnly]
        public async Task ReloadConfig()
        {
            //await Context.Message.AddReactionAsync(new Emoji("👌"));
            _config.ReloadConfig(true);
            await Context.Message.ReplyAsync($":ok_hand: `{StringResourceHandler.GetTextStatic("Admin", "reloadconfig")}`");
        }
    }
}
