using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using DaveBot.Services;
using DaveBot.Common.ModuleBehaviors;

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
            //TODO: Actually add stuff here.
            return false;
        }
    }
}
