using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using DaveBot.Services;
using NLog;
using DaveBot.Common;
using System.Threading;
using System;

namespace DaveBot.Modules.Core.Services
{
    public class GameRotationService : IDaveBotService
    {
        private readonly Timer _t;

        private readonly DiscordShardedClient _client;
        private readonly IBotConfiguration _config;

        private readonly DaveRNG _rng;

        public GameRotationService(DiscordShardedClient client, IBotConfiguration config)
        {
            _client = client;
            _config = config;
            _rng = new DaveRNG();
            _t = new Timer(NextPlayingStatus, null, 30000, 30000);
        }

        private async void NextPlayingStatus(object state)
        {
            try
            {
                if (_config.RotatePlayingStatuses && (_config.PlayingStatuses.Length > 0))
                    await _client.SetGameAsync(_config.PlayingStatuses[_rng.Next(_config.PlayingStatuses.Length - 1)]);
            }catch(Exception e)
            {
                LogManager.GetCurrentClassLogger().Error($"Couldn't update playing status: {e.Message}");
            }
        }

        public void SetRotationInterval(int newInterval)
        {
            _t.Change(newInterval, newInterval);
        }
    }
}
