using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using DaveBot.Common;
using DaveBot.Modules.CustomReactions.Services;
using DaveBot.Services;
using System.Collections.Generic;
using DaveBot.Common.Attributes;
using System;

namespace DaveBot.Modules.CustomReactions
{
    [Name("Custom Reactions")]
    public class CustomReactionsModule : DaveBotTopModuleBase<CustomReactionService>
    {

        private readonly IBotConfiguration _config;

        public CustomReactionsModule(IBotConfiguration config)
        {
            _config = config;
        }

        [Command("lcr")]
        [Summary("Lists all the custom reactions stored in the bot.")]
        public async Task ListCustomReactions()
        {
            var pleasewait = Context.Channel.EnterTypingState();
            string desc = "";
            int crcount = 1;
            foreach (var cr in _config.CustomReactions)
            {
                if (desc.Length < 800)
                    desc += "• " + cr.Key + '\n';
                else
                {
                    desc += StringResourceHandler.GetTextStatic("CustomReactions", "ShowCustomReaction_more", cr.Value.Count - crcount);
                    break;
                }
                crcount++;
            }
            if (_config.CustomReactions.Count == 0)
                desc += StringResourceHandler.GetTextStatic("CustomReactions", "lcr_noCustomReactions");
            else if(_config.CustomReactions.Count == 1)
                desc += StringResourceHandler.GetTextStatic("CustomReactions", "lcr_TotalCountOne");
            else
                desc += StringResourceHandler.GetTextStatic("CustomReactions", "lcr_TotalCountMultiple", _config.CustomReactions.Count);
            EmbedBuilder eb = new EmbedBuilder()
                .WithTitle("📃 " + StringResourceHandler.GetTextStatic("CustomReactions", "ListCustomReactions"))
                .WithDescription(desc)
                .WithColor(Color.Green);
            await Context.Message.AddReactionAsync(new Emoji("📃"));
            pleasewait.Dispose();
            await ReplyAsync(Context.Message.Author.Mention, false, eb.Build());
        }

        [Command("scr")]
        [Summary("Shows all the responses for a specified custom reaction")]
        public async Task ShowCustomReaction([Remainder] [Summary("Custom reaction to view")] string ikey)
        {
            var pleasewait = Context.Channel.EnterTypingState();
            int matches = 0;
            string inputkey = ikey.Trim().ToLower();
            EmbedBuilder eb = new EmbedBuilder();
            foreach (var cr in _config.CustomReactions)
            {
                if(cr.Key.Trim().ToLower().Equals(inputkey))
                {
                    string desc = "";
                    string title = $"**{StringResourceHandler.GetTextStatic("CustomReactions", "trigger")}** {cr.Key}";
                    int respnum = 1;
                    foreach(var response in cr.Value.ToArray())
                    {
                        if (desc.Length < 850)
                            desc += $"***{StringResourceHandler.GetTextStatic("CustomReactions", "response", respnum)}*** {response}\n";
                        else
                        {
                            desc += StringResourceHandler.GetTextStatic("CustomReactions", "ShowCustomReaction_more", cr.Value.Count-respnum);
                            break;
                        }
                        respnum++;
                    }
                    desc = desc.Trim();
                    if (matches > 0)
                        title += $" ({matches})";
                    eb.AddField(title, desc);
                    matches++;
                }
            }
            try
            {
                if (matches == 0)
                {
                    eb.WithTitle("🚫 " + StringResourceHandler.GetTextStatic("CustomReactions", "ShowCustomReaction_noResults"))
                        .WithDescription(StringResourceHandler.GetTextStatic("CustomReactions", "ShowCustomReaction_noResults_desc", ikey))
                        .WithColor(Color.Red);
                    await Context.Message.AddReactionAsync(new Emoji("🚫"));
                }
                else
                {
                    eb.WithColor(Color.Green);
                    if (matches > 1)
                        eb.WithTitle("📂 " + StringResourceHandler.GetTextStatic("CustomReactions", "ShowCustomReaction_multipleResults"));
                    else
                        eb.WithTitle("📂 " + StringResourceHandler.GetTextStatic("CustomReactions", "ShowCustomReaction"));
                    await Context.Message.AddReactionAsync(new Emoji("📂"));
                }
            }
            catch(Exception)
            {
                pleasewait.Dispose();
                throw;
            }
            pleasewait.Dispose();
            await ReplyAsync(Context.Message.Author.Mention, false, eb.Build());
        }
        //TODO: Add pal:acr (add custom reaction), pal:dcr (delete custom reaction), and pal:ecr (edit custom reaction)
        [Command("acr")]
        [Summary("Adds a custom reaction")]
        [OwnerOnly]
        public async Task AddCustomReaction(string trigger, [Remainder] string response)
        {
            var pleasewait = Context.Channel.EnterTypingState();
            int respnum = 1;
            bool keyAlreadyExists = false;
            foreach (var existingKey in _config.CustomReactions.Keys)
            {
                if (existingKey.ToLowerInvariant() == trigger.ToLowerInvariant())
                {
                    keyAlreadyExists = true;
                    _config.CustomReactions[existingKey].Add(response);
                    respnum = _config.CustomReactions[existingKey].Count;
                    break;
                }
            }
            if (!keyAlreadyExists)
            {
                _config.CustomReactions.Add(trigger, new List<string>());
                _config.CustomReactions[trigger].Add(response);
            }
            _config.SaveConfig(true);
            _config.ReloadConfig(false);
            var replyEmbed = new EmbedBuilder()
                .WithTitle("✨ " + StringResourceHandler.GetTextStatic("CustomReactions", "AddCustomReaction"))
                .AddField(StringResourceHandler.GetTextStatic("CustomReactions", "trigger"), trigger)
                .AddField(StringResourceHandler.GetTextStatic("CustomReactions", "response", respnum), response)
                .Build();
            await Context.Message.AddReactionAsync(new Emoji("✨"));
            pleasewait.Dispose();
            await ReplyAsync(Context.User.Mention, false, replyEmbed);
        }
    }
}
