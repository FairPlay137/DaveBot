using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using System.Diagnostics;
using System.Linq;
using DaveBot.Common;
using DaveBot.Services;
using DaveBot.Common.Attributes;

namespace DaveBot.Modules
{
    [Name("Utilities")]
    public class UtilsModule : DaveBotTopModuleBase
    {
        private readonly DaveBot _bot;

        private readonly IBotConfiguration _config;

        private readonly string[] defaultPingLocations = { //TODO: move this into the config too
            "127.0.0.1",
            "localhost",
            "Google",
            "a random server",
            "fairplay137.net",
            "therofl98.co",
            "2006, to warn them about Windows Vista's then-upcoming release",
            "@everyone",
            "[insert witty ping location here]",
            "davemadson's coffee maker",
            "YouTube",
            "Microsoft Sam",
            "`undefined`",
            "the ROFLcopter",
            "the WB shield",
            "uh... well, I can't remember at this point",
            "the Viacom V of Doom",
            "the \"Television\" text from that blue mountain logo",
            "the real davemadson",
            "Radar Overseer Scotty"
        };

        public UtilsModule(DaveBot bot)
        {
            _bot = bot;
            _config = bot.Configuration;
        }

        [Command("ping")]
        [Summary("Checks the bot's ping time.")]
        public async Task Ping()
        {
            await Context.Message.AddReactionAsync(new Emoji("🏓"));
            var pleasewait = Context.Channel.EnterTypingState();
            var pingwaitmsg = StringResourceHandler.GetTextStatic("Utils", "ping_wait");
            var msg = await Context.Channel.SendMessageAsync("🏓 " + pingwaitmsg).ConfigureAwait(false);
            var sw = Stopwatch.StartNew();
            await msg.DeleteAsync();
            sw.Stop();
            var random = new DaveRNG();
            var subtitleText = StringResourceHandler.GetTextStatic("Utils", "ping_subtitle" + random.Next(1, 5));
            var footerText = StringResourceHandler.GetTextStatic("Utils", "ping_footer1");
            if(sw.ElapsedMilliseconds > 150)
                footerText = StringResourceHandler.GetTextStatic("Utils", "ping_footer2");
            if (sw.ElapsedMilliseconds > 300)
                footerText = StringResourceHandler.GetTextStatic("Utils", "ping_footer3");
            if (sw.ElapsedMilliseconds > 800)
                footerText = StringResourceHandler.GetTextStatic("Utils", "ping_footer4");
            if (sw.ElapsedMilliseconds > 3000)
                footerText = StringResourceHandler.GetTextStatic("Utils", "ping_footer5");
            pleasewait.Dispose();
            await ReplyAsync(Context.User.Mention, false, new EmbedBuilder()
                .WithTitle("🏓 " + StringResourceHandler.GetTextStatic("Utils", "ping_title"))
                .WithDescription(subtitleText+'\n'+StringResourceHandler.GetTextStatic("Utils", "ping_pingtime", sw.ElapsedMilliseconds, defaultPingLocations[random.Next(defaultPingLocations.Length)]))
                .WithFooter(footerText)
                .WithColor(Color.Blue)
                .Build());
        }
        [Command("invite")]
        [Summary("Gets the invite link for this bot.")]
        public async Task Invite()
        {
            await Context.Message.AddReactionAsync(new Emoji("👌"));
            await ReplyAsync($"{Context.User.Mention} - {StringResourceHandler.GetTextStatic("Utils", "invite")} https://discordapp.com/oauth2/authorize?client_id={Context.Client.CurrentUser.Id}&permissions=8&scope=bot%20applications.commands");
        }
        [Command("stats")]
        [Summary("Gets this bot's stats.")]
        public async Task Stats()
        {
            await Context.Message.AddReactionAsync(new Emoji("👌"));
            TimeSpan uptime = new TimeSpan(DateTime.Now.Ticks - _bot.StartTime.Ticks);
            await ReplyAsync(Context.User.Mention, false, new EmbedBuilder()
                .WithTitle(StringResourceHandler.GetTextStatic("Utils", "stats_title", _config.BotName))
                .WithDescription((_config.BotName == "DaveBot") ?
                StringResourceHandler.GetTextStatic("Utils", "stats_descriptionPublic")
                :
                (_config.BotOwnerID == 287675563446108160) ?
                StringResourceHandler.GetTextStatic("Utils", "stats_description2", _config.BotName)
                :
                StringResourceHandler.GetTextStatic("Utils", "stats_description", _config.BotName))
                .WithAuthor($"{Context.Client.CurrentUser.Username} v{typeof(Program).Assembly.GetName().Version}",Context.Client.CurrentUser.GetAvatarUrl())
                .AddField(StringResourceHandler.GetTextStatic("Utils", "stats_guilds"),Context.Client.Guilds.Count,true)
                .AddField(StringResourceHandler.GetTextStatic("Utils", "stats_uptime"), uptime.ToString(), true)
                .WithColor(Color.Blue)
                .Build());
        }
        [Command("serverinfo")]
        [Summary("Retrieves the server's information")]
        [CannotUseInDMs]
        public async Task GuildInfo()
        {
            var featureList = StringResourceHandler.GetTextStatic("Utils", "sinfo_noFeatures");
            // old code which doesn't work with D.NET 3
            /*if(Context.Guild.Features.Count > 0)
            {
                featureList = Context.Guild.Features.Aggregate("", (current, feature) => current + ("• " + feature + '\n'));
            }*/
            featureList = "NOT YET IMPLEMENTED";
            var inviteLinks = await Context.Guild.GetInvitesAsync().ConfigureAwait(false);
            var userCount = 0;
            var botCount = 0;
            foreach(var user in Context.Guild.Users)
            {
                if (user.IsBot)
                    botCount++;
                else
                    userCount++;
            }
            var vl = Context.Guild.VerificationLevel;
            var vli = vl switch
            {
                VerificationLevel.Low => 1,
                VerificationLevel.Medium => 2,
                VerificationLevel.High => 3,
                VerificationLevel.Extreme => 4,
                _ => 0
            };
            await ReplyAsync(Context.User.Mention, false, new EmbedBuilder()
                .WithTitle(Context.Guild.Name)
                .AddField(StringResourceHandler.GetTextStatic("Utils", "sinfo_id"),Context.Guild.Id,true)
                .AddField(StringResourceHandler.GetTextStatic("Utils", "sinfo_created"), Context.Guild.CreatedAt, true)
                .AddField(StringResourceHandler.GetTextStatic("Utils", "sinfo_owner"), $"@{Context.Guild.Owner.Username}#{Context.Guild.Owner.Discriminator} ({Context.Guild.Owner.Id})", true)
                .AddField(StringResourceHandler.GetTextStatic("Utils", "sinfo_users"), $"{Context.Guild.MemberCount} {StringResourceHandler.GetTextStatic("Utils", "sinfo_userbotratio",userCount,botCount)}", true)
                .AddField(StringResourceHandler.GetTextStatic("Utils", "sinfo_categories"), Context.Guild.CategoryChannels.Count, true)
                .AddField(StringResourceHandler.GetTextStatic("Utils", "sinfo_textchannels"), Context.Guild.TextChannels.Count, true)
                .AddField(StringResourceHandler.GetTextStatic("Utils", "sinfo_voicechannels"), Context.Guild.VoiceChannels.Count, true)
                .AddField(StringResourceHandler.GetTextStatic("Utils", "sinfo_roles"), Context.Guild.Roles.Count, true)
                .AddField(StringResourceHandler.GetTextStatic("Utils", "sinfo_verificationlevel"), StringResourceHandler.GetTextStatic("Utils", $"sinfo_verificationlevel_{vli}"), true)
                .AddField(StringResourceHandler.GetTextStatic("Utils", "sinfo_features"), featureList, true)
                .AddField(StringResourceHandler.GetTextStatic("Utils", "sinfo_invites"), inviteLinks.Count, true)
                .AddField(StringResourceHandler.GetTextStatic("Utils", "sinfo_customemotes"), Context.Guild.Emotes.Count, true)
                .WithThumbnailUrl(Context.Guild.IconUrl)
                .WithColor(Color.Blue)
                .Build());
        }


    }
}
