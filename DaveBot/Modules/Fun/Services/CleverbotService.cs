using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using DaveBot.Services;
using DaveBot.Common.ModuleBehaviors;
using DaveBot.Common;

namespace DaveBot.Modules.Fun.Services
{
    public class CleverbotService : IPreXBlockerExecutor, IDaveBotService
    {
        private readonly DiscordShardedClient _client;

        private readonly IBotConfiguration _config;

        public CleverbotService(DiscordShardedClient client, IBotConfiguration config)
        {
            _client = client;
            _config = config;
        }

        public async Task<bool> TryExecuteEarly(DiscordSocketClient client, IGuild guild, IUserMessage msg)
        {
            if (msg.Content.StartsWith(client.CurrentUser.Mention) && IsCleverbotChannel(msg.Channel))
            {
                await msg.Channel.SendMessageAsync($":speech_balloon: `{StringResourceHandler.GetTextStatic("Fun", "cleverbot_comingSoon")}`");
            }
            return false;
        }

        private bool IsCleverbotChannel(IMessageChannel channel)
        {
            try
            {
                return _config.CleverbotChannels.Any(chnid => channel.Id == chnid);
            }catch(Exception)
            {
                return false;
            }
        }
    }
}
