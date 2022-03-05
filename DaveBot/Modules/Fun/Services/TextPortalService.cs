#if !PUBLIC_BUILD
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using DaveBot.Services;
using DaveBot.Common.ModuleBehaviors;
using System.Collections.Generic;
using NLog;
using DaveBot.Common;
using System.Threading;
using System;
using System.Diagnostics;
using System.Linq;

namespace DaveBot.Modules.Fun.Services
{
    public class TextPortalService : IPostXExecutor, IDaveBotService
    {
        private readonly DiscordShardedClient _client;

        private readonly IBotConfiguration _config;

        public Dictionary<ITextChannel, ITextChannel> textPortals = new();

        public int maxTextPortals = 64; //TODO: Make this configurable
        public int timeoutPeriodSecs = 45; //TODO: Make this configurable
        public int answerTimeoutPeriodSecs = 60; //TODO: Make this configurable

        private readonly Timer _t;

        public TextPortalService(DiscordShardedClient client, IBotConfiguration config)
        {
            _client = client;
            _config = config;
            _client.ChannelDestroyed += ChannelDeleteHandler;
            //_client.UserIsTyping += TypingHandler;
            _t = new Timer(TimerTick, null, 1000, 1000);
        }

        private async void TimerTick(object state)
        {
            var num = 0;
            if (textPortals == null) return;
            try
            {
                var tpc = textPortals;
                foreach (var tp in tpc)
                {
                    if (tp.Value != null) //make sure we ignore non-paired portals
                    {
                        if (tp.Key.Id == tp.Value.Id)
                        {
                            LogManager.GetCurrentClassLogger().Warn($"Can't have a text portal pointing to the same channel! Deleting text portal {num}...");
                            textPortals.Remove(tp.Key);
                        }
                        else
                        {
                            var m1e = await tp.Key.GetMessagesAsync(1).FlattenAsync().ConfigureAwait(false);
                            var m2e = await tp.Value.GetMessagesAsync(1).FlattenAsync().ConfigureAwait(false);
                            var m1 = m1e.GetEnumerator(); m1.MoveNext();
                            var m2 = m2e.GetEnumerator(); m2.MoveNext();
                            Debug.Assert(m1.Current != null, "m1.Current != null");
                            Debug.Assert(m2.Current != null, "m2.Current != null");
                            var mostRecentMessage = (m1.Current.Timestamp.Ticks > m2.Current.Timestamp.Ticks) ?
                                m1.Current : m2.Current;
                            m1.Dispose(); m2.Dispose(); //these aren't needed anymore since we're finished comparing them
                            LogManager.GetCurrentClassLogger().Debug($"{DateTime.UtcNow.Ticks - mostRecentMessage.Timestamp.Ticks} ticks since last message");
                            if ((DateTime.UtcNow.Ticks - mostRecentMessage.Timestamp.Ticks) > (timeoutPeriodSecs * 10000000))
                            {
                                try
                                {
                                    await tp.Key.SendMessageAsync($":telephone_receiver: `{StringResourceHandler.GetTextStatic("Fun", "textportal_connectionLost_timeout", timeoutPeriodSecs)}`");
                                    await tp.Value.SendMessageAsync($":telephone_receiver: `{StringResourceHandler.GetTextStatic("Fun", "textportal_connectionLost_timeout", timeoutPeriodSecs)}`");
                                }
                                catch (Exception e)
                                {
                                    LogManager.GetCurrentClassLogger().Warn($"Whoa, something happened and I couldn't send the timeout message! (perhaps a lack of permissions?)");
                                    LogManager.GetCurrentClassLogger().Warn(e);
                                }
                                textPortals.Remove(tp.Key);
                            }
                        }
                    }
                    num++;
                }
            }catch(Exception e)
            {
                LogManager.GetCurrentClassLogger().Warn("An error occurred in the timeout routine.");
                LogManager.GetCurrentClassLogger().Warn(e);
            }
        }

        // TODO: refactor this for Discord.NET 3
        private async Task TypingHandler(SocketUser arg1, ISocketMessageChannel arg2)
        {
            if (arg1.IsBot)
                return;
            ITextChannel targetChannel = null;
            var foundChannel = 0;
            var num = 0;
            if (textPortals != null)
            {
                foreach (var (key, value) in textPortals)
                {
                    if (value != null) //make sure we ignore non-paired portals
                    {
                        if (key.Id == value.Id)
                        {
                            LogManager.GetCurrentClassLogger().Warn($"Can't have a text portal pointing to the same channels! Deleting text portal {num}..");
                            textPortals.Remove(key);
                        }
                        else
                        {
                            if (key.Id == arg2.Id)
                            {
                                targetChannel = value;
                                foundChannel++;
                            }
                            if (value.Id == arg2.Id)
                            {
                                targetChannel = key;
                                foundChannel++;
                            }
                        }
                    }
                    num++;
                }
                if ((foundChannel == 1) && (targetChannel != null))
                    await targetChannel.TriggerTypingAsync();
            }
        }

        private async Task ChannelDeleteHandler(SocketChannel arg)
        {
            ITextChannel targetChannel = null;
            var foundChannel = 0;
            var num = 0;
            if (textPortals != null)
            {
                foreach (var tp in textPortals)
                {
                    if (tp.Value != null) //make sure we ignore non-paired portals
                    {
                        if (tp.Key.Id == tp.Value.Id)
                        {
                            LogManager.GetCurrentClassLogger().Warn($"Can't have a text portal pointing to the same channel! Deleting text portal {num}..");
                            textPortals.Remove(tp.Key);
                        }
                        else
                        {
                            if (tp.Key.Id == arg.Id)
                            {
                                targetChannel = tp.Value;
                                foundChannel++;
                            }
                            if (tp.Value.Id == arg.Id)
                            {
                                targetChannel = tp.Key;
                                foundChannel++;
                            }
                        }
                    }
                    else
                    {
                        if (tp.Key.Id == arg.Id)
                        {
                            LogManager.GetCurrentClassLogger().Debug($"Freeing up unneeded slot {num}...");
                            textPortals.Remove(tp.Key);
                        }
                    }
                    num++;
                }
                if ((foundChannel == 1) && (targetChannel != null)) //Was the channel that was deleted part of an existing text portal?
                {
                    textPortals.Remove((ITextChannel)arg); //To prevent problems later on
                    await targetChannel.SendMessageAsync($":telephone_receiver: `{StringResourceHandler.GetTextStatic("Fun", "textportal_connectionLost_deletedChannel")}`");
                }
            }
        }

        public bool FindAndOccupyOpenSlot(ITextChannel textChannel)
        {
            foreach (var tp in textPortals.Where(tp => tp.Value == null))
            {
                textPortals[tp.Key] = textChannel;
                return true;
            }

            return false;
        }

        public async Task LateExecute(DiscordSocketClient client, IGuild guild, IUserMessage msg)
        {
            if (msg.Author.IsBot)
                return;
            ITextChannel targetChannel = null;
            var foundChannel = 0;
            var num = 0;
            if (textPortals != null)
            {
                foreach (var (key, value) in textPortals)
                {
                    if (value != null) //make sure we ignore non-paired portals
                    {
                        if (key.Id == value.Id)
                        {
                            LogManager.GetCurrentClassLogger().Warn($"Can't have a text portal pointing to the same channel! Deleting text portal {num}..");
                            textPortals.Remove(key);
                        }
                        else
                        {
                            if (key.Id == msg.Channel.Id)
                            {
                                targetChannel = value;
                                foundChannel++;
                            }
                            if (value.Id == msg.Channel.Id)
                            {
                                targetChannel = key;
                                foundChannel++;
                            }
                        }
                    }
                    num++;
                }
                if ((foundChannel == 1) && (targetChannel != null) && !msg.Content.StartsWith(_config.DefaultPrefix))
                {
                    var eb = new EmbedBuilder()
                        .WithDescription($"**@{msg.Author.Username}#{msg.Author.Discriminator}** :speech_balloon: {msg.Content}");
                    await targetChannel.SendMessageAsync("", false, eb.Build());
                }
            }
        }
    }
}
#endif