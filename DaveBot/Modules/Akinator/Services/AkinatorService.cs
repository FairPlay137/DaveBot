using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using DaveBot.Services;
using DaveBot.Common.ModuleBehaviors;
using AkiNet;

namespace DaveBot.Modules.Akinator.Services
{
    public class AkinatorService : IDaveBotService
    {
        private readonly DiscordShardedClient _client;

        private readonly IBotConfiguration _config;

        private Client _akiClient;

        public AkinatorService(DiscordShardedClient client, IBotConfiguration config)
        {
            _client = client;
            _config = config;
        }
    }
}
