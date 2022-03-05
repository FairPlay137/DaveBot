using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using DaveBot.Common;
using DaveBot.Common.Attributes;

namespace DaveBot.Modules
{
    [Name("Moderation")]
    public class ModerationModule : DaveBotTopModuleBase
    {
        [Command("kick")]
        [Summary("Kicks a specified user from the server.")]
        [RequireContext(ContextType.Guild)]
        [RequireBotPermission(GuildPermission.KickMembers)]
        [RequireUserPermission(GuildPermission.KickMembers)]
        [CannotUseInDMs]
        public async Task Kick([Summary("User to kick")] IGuildUser target, [Remainder] string reason = null)
        {
            if(target.Id == Context.Message.Author.Id)
            {
                await Context.Message.AddReactionAsync(new Emoji("⛔"));
                await ReplyAsync($":no_entry_sign: `{StringResourceHandler.GetTextStatic("Moderation", "cannotKickSelf")}`").ConfigureAwait(false);
                return;
            }
            try
            {
                var userdm = await target.CreateDMChannelAsync();
                var dmembed = new EmbedBuilder()
                    .WithColor(Color.Red)
                    .WithTitle(StringResourceHandler.GetTextStatic("Moderation", "dm_reason"))
                    .WithDescription(reason ?? StringResourceHandler.GetTextStatic("Moderation", "dm_NoReasonSpecified"))
                    .WithFooter($"{StringResourceHandler.GetTextStatic("Moderation", "dm_by", StringResourceHandler.GetTextStatic("Moderation", "dm_kicked"))} @{Context.User.Username}#{Context.User.Discriminator}")
                    .Build();
                await userdm.SendMessageAsync($":anger: {StringResourceHandler.GetTextStatic("Moderation", "dm_kick_header", Context.Guild.Name)}", false, dmembed);
            }
            catch (Exception e)
            {
                await ReplyAsync(":no_entry_sign: "+StringResourceHandler.GetTextStatic("Moderation", "DMFailed", e.Message));
            }
            await Context.Message.AddReactionAsync(new Emoji("👢"));
            await target.KickAsync(reason).ConfigureAwait(false);
            await ReplyAsync($":boot: `{StringResourceHandler.GetTextStatic("Moderation", "kick", $"@{target.Username}#{target.Discriminator}")}`").ConfigureAwait(false);
        }
        [Command("ban")]
        [Summary("Bans a specified user from the server.")]
        [RequireContext(ContextType.Guild)]
        [RequireBotPermission(GuildPermission.BanMembers)]
        [RequireUserPermission(GuildPermission.BanMembers)]
        [CannotUseInDMs]
        public async Task Ban([Summary("User to ban")] IGuildUser target, [Remainder] string reason = null)
        {
            if (target.Id == Context.Message.Author.Id)
            {
                await Context.Message.AddReactionAsync(new Emoji("⛔"));
                await ReplyAsync($":no_entry_sign: `{StringResourceHandler.GetTextStatic("Moderation", "cannotBanSelf")}`").ConfigureAwait(false);
                return;
            }
            try
            {
                var userdm = await target.CreateDMChannelAsync();
                var dmembed = new EmbedBuilder()
                    .WithColor(Color.Red)
                    .WithTitle(StringResourceHandler.GetTextStatic("Moderation", "dm_reason"))
                    .WithDescription(reason ?? StringResourceHandler.GetTextStatic("Moderation", "dm_NoReasonSpecified"))
                    .WithFooter($"{StringResourceHandler.GetTextStatic("Moderation", "dm_by", StringResourceHandler.GetTextStatic("Moderation", "dm_banned"))} @{Context.User.Username}#{Context.User.Discriminator}")
                    .Build();
                await userdm.SendMessageAsync($":anger: {StringResourceHandler.GetTextStatic("Moderation", "dm_ban_header", Context.Guild.Name)}",false,dmembed);
            }
            catch(Exception e)
            {
                await ReplyAsync(":no_entry_sign: " + StringResourceHandler.GetTextStatic("Moderation", "DMFailed", e.Message));
            }
            await Context.Message.AddReactionAsync(new Emoji("🔨"));
            await Context.Guild.AddBanAsync(target,0,reason).ConfigureAwait(false);
            await ReplyAsync($":hammer: `{StringResourceHandler.GetTextStatic("Moderation", "ban", $"@{target.Username}#{target.Discriminator}")}`").ConfigureAwait(false);
        }

        [Command("prune")]
        [Summary("Prunes messages from the current channel.")]
        [RequireBotPermission(GuildPermission.ManageMessages)]
        [RequireUserPermission(GuildPermission.ManageMessages)]
        public async Task Prune(int limits)
        {
            if (limits < 0)
            {
                await Context.Message.AddReactionAsync(new Emoji("⛔"));
                await ReplyAsync($":no_entry_sign: `{StringResourceHandler.GetTextStatic("Moderation", "negativePruneImpossible")}`").ConfigureAwait(false);
                return;
            }
            var messages = await Context.Channel.GetMessagesAsync(limits + 1).FlattenAsync();
            var channel = (ITextChannel)Context.Channel;
            await channel.DeleteMessagesAsync(messages);
            
            var SelfDestructingResultMessage = await ReplyAsync($":white_check_mark: `{StringResourceHandler.GetTextStatic("Moderation", "prune", limits)}`").ConfigureAwait(false);
            await Task.Delay(5000); //TODO: There *has* to be a better way to do this.
            await SelfDestructingResultMessage.DeleteAsync();
        }
    }
}
