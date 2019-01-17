﻿using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using DaveBot.Services;
using DaveBot.Common.ModuleBehaviors;
using System.Collections.Generic;
using NLog;
using DaveBot.Common;

namespace DaveBot.Modules.Fun.Services
{
    public class TextPortalService : IPostXExecutor, IDaveBotService
    {
        private readonly DiscordShardedClient _client;

        private readonly IBotConfiguration _config;

        public Dictionary<ITextChannel, ITextChannel> textPortals = new Dictionary<ITextChannel, ITextChannel>();

        public int maxTextPortals = 64; //TODO: Make this configurable

        public TextPortalService(DiscordShardedClient client, IBotConfiguration config)
        {
            _client = client;
            _config = config;
            _client.ChannelDestroyed += _client_ChannelDestroyed;
            _client.UserIsTyping += _client_UserIsTyping;
        }

        private async Task _client_UserIsTyping(SocketUser arg1, ISocketMessageChannel arg2)
        {
            if (arg1.IsBot)
                return;
            ITextChannel targetChannel = null;
            int foundChannel = 0;
            int num = 0;
            if (textPortals != null)
            {
                foreach (var tp in textPortals)
                {
                    if (tp.Value != null) //make sure we ignore non-paired portals
                    {
                        if (tp.Key.Id == tp.Value.Id)
                        {
                            LogManager.GetCurrentClassLogger().Warn($"Can't have a text portal pointing to the same channels! Deleting text portal {num}..");
                            textPortals.Remove(tp.Key);
                        }
                        else
                        {
                            if (tp.Key.Id == arg2.Id)
                            {
                                targetChannel = tp.Value;
                                foundChannel++;
                            }
                            if (tp.Value.Id == arg2.Id)
                            {
                                targetChannel = tp.Key;
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

        private async Task _client_ChannelDestroyed(SocketChannel arg)
        {
            ITextChannel targetChannel = null;
            int foundChannel = 0;
            int num = 0;
            if (textPortals != null)
            {
                foreach (var tp in textPortals)
                {
                    if (tp.Value != null) //make sure we ignore non-paired portals
                    {
                        if (tp.Key.Id == tp.Value.Id)
                        {
                            LogManager.GetCurrentClassLogger().Warn($"Can't have a text portal pointing to the same channels! Deleting text portal {num}..");
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
                if ((foundChannel == 1) && (targetChannel != null))
                {
                    textPortals.Remove((ITextChannel)arg);
                    await targetChannel.SendMessageAsync($":telephone_receiver: `{StringResourceHandler.GetTextStatic("Fun", "textportal_connectionLost_deletedChannel")}`");
                }
            }
        }

        public bool FindAndOccupyOpenSlot(ITextChannel textChannel)
        {
            foreach (var tp in textPortals)
            {
                if(tp.Value == null)
                {
                    textPortals[tp.Key] = textChannel;
                    return true;
                }
            }
            return false;
        }

        public async Task LateExecute(DiscordSocketClient client, IGuild guild, IUserMessage msg)
        {
            if (msg.Author.IsBot)
                return;
            ITextChannel targetChannel = null;
            int foundChannel = 0;
            int num = 0;
            if (textPortals != null)
            {
                foreach (var tp in textPortals)
                {
                    if (tp.Value != null) //make sure we ignore non-paired portals
                    {
                        if (tp.Key.Id == tp.Value.Id)
                        {
                            LogManager.GetCurrentClassLogger().Warn($"Can't have a text portal pointing to the same channels! Deleting text portal {num}..");
                            textPortals.Remove(tp.Key);
                        }
                        else
                        {
                            if (tp.Key.Id == msg.Channel.Id)
                            {
                                targetChannel = tp.Value;
                                foundChannel++;
                            }
                            if (tp.Value.Id == msg.Channel.Id)
                            {
                                targetChannel = tp.Key;
                                foundChannel++;
                            }
                        }
                    }
                    num++;
                }
                if ((foundChannel == 1) && (targetChannel != null) && !msg.Content.StartsWith(_config.DefaultPrefix))
                {
                    EmbedBuilder eb = new EmbedBuilder()
                        .WithDescription($"**@{msg.Author.Username}#{msg.Author.Discriminator}** :speech_balloon:\n{msg.Content}");
                    await targetChannel.SendMessageAsync("", false, eb.Build());
                }
            }
        }
    }
}
