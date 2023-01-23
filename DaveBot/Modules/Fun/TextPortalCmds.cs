#if !PUBLIC_BUILD
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using DaveBot.Services;
using DaveBot.Modules.Fun.Services;
using DaveBot.Common;
using DaveBot.Common.Attributes;
using System.Linq;
using NLog;

// This module is not enabled on the public instance due to its reliance on the Message Content intent, which DaveBot has not been granted.

namespace DaveBot.Modules.Fun
{
    public partial class FunModule
    {
        [Group]
        [Name("Text Portal")]
        public partial class TextPortalCmds : DaveBotSubModuleBase
        {
            private readonly IBotConfiguration _config; //TODO: Add some parameters allowing for the Text Portal feature to be more configurable.
            private readonly TextPortalService _textportals;

            public TextPortalCmds(IBotConfiguration config, TextPortalService textportals)
            {
                _config = config;
                _textportals = textportals;
            }

            [Command("textportal")]
            [Summary("Opens or closes a text portal")]
            [CannotUseInDMs]
            public async Task TextPortal()
            {
                if (!_config.EnableTextPortals)
                {
                    LogManager.GetCurrentClassLogger().Info("_config.EnableTextPortals appears to be false.");
                    throw new CommandUnsuccessfulException(StringResourceHandler.GetTextStatic("Fun", "textportal_error_featureDisabled"));
                }

                if (_textportals.textPortals.ContainsKey((ITextChannel)Context.Channel))
                {
                    if (_textportals.textPortals[(ITextChannel)Context.Channel] != null)
                    {
                        await _textportals.textPortals[(ITextChannel)Context.Channel].SendMessageAsync($":telephone_receiver: `{StringResourceHandler.GetTextStatic("Fun", "textportal_connectionLost_otherSideClose")}`");
                        await ReplyAsync($":telephone_receiver: `{StringResourceHandler.GetTextStatic("Fun", "textportal_connectionLost_generic")}`");
                    }else
                        await ReplyAsync($":telephone_receiver: `{StringResourceHandler.GetTextStatic("Fun", "textportal_connectionLost_noResponse")}`");
                    _textportals.textPortals.Remove((ITextChannel)Context.Channel);
                }
                else
                {
                    if (_textportals.textPortals.ContainsValue((ITextChannel)Context.Channel))
                    {
                        ITextChannel channel = null;
                        foreach (var tp in _textportals.textPortals.Where(tp => tp.Value.Id == Context.Channel.Id))
                            channel = tp.Key;
                        if (channel != null)
                        {
                            await channel.SendMessageAsync($":telephone_receiver: `{StringResourceHandler.GetTextStatic("Fun", "textportal_connectionLost_otherSideClose")}`");
                            await ReplyAsync($":telephone_receiver: `{StringResourceHandler.GetTextStatic("Fun", "textportal_connectionLost_generic")}`");
                            _textportals.textPortals.Remove(channel);
                        }
                    }
                    else
                    {
                        if (_textportals.FindAndOccupyOpenSlot((ITextChannel)Context.Channel))
                        {
                            ITextChannel channel = null;
                            foreach (var tp in _textportals.textPortals.Where(tp => tp.Value.Id == Context.Channel.Id))
                                channel = tp.Key;
                            await ReplyAsync($":telephone_receiver: `{StringResourceHandler.GetTextStatic("Fun", "textportal_connectionEstablished")}`");
                            if (channel != null)
                                await channel.SendMessageAsync(
                                    $":telephone_receiver: `{StringResourceHandler.GetTextStatic("Fun", "textportal_connectionEstablished")}`");
                            else
                                await ReplyAsync("**UNEXPECTED ERROR - TARGET CHANNEL IS NULL**");
                        }
                        else
                        {
                            if(_textportals.textPortals.Count >= _textportals.maxTextPortals)
                                throw new CommandUnsuccessfulException(StringResourceHandler.GetTextStatic("Fun", "textportal_error_limitReached", _textportals.maxTextPortals));
                            _textportals.textPortals.Add((ITextChannel)Context.Channel, null);
                            await ReplyAsync($":telephone_receiver: `{StringResourceHandler.GetTextStatic("Fun", "textportal_waiting")}`");
                        }
                    }
                }
            }
        }
    }
}
#endif