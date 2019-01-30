using System.Collections.Generic;

namespace DaveBot.Services
{
    public interface IBotConfiguration
    {
        string BotName { get; }
        string BotToken { get; }
        ulong BotOwnerID { get; }

        int TotalShards { get; set; }

        string DefaultPlayingString { get; }
        string DefaultPrefix { get; set; }

        bool VerboseErrors { get; set; }

        string[] EightBallResponses { get; }

        Dictionary<string, List<string>> CustomReactions { get; set; }

        bool RotatePlayingStatuses { get; set; }
        string[] PlayingStatuses { get; set; }

        string GoogleAPIKey { get; }

        ulong[] CleverbotChannels { get; set; }

        bool EnableTextPortals { get; set; }

        bool ReloadConfig(bool verbose);
        bool SaveConfig(bool verbose);
    }
}
