using System.Collections.Generic;
using Newtonsoft.Json;

namespace DaveBot.Common
{
    public struct ConfigJsonStructure
    {
        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("prefix")]
        public string CommandPrefix { get; set; }

        [JsonProperty("totalShards")]
        public int TotalShards { get; set; }

        [JsonProperty("defaultplaying")]
        public string DefaultPlayingString { get; set; }

        [JsonProperty("ownerid")]
        public ulong OwnerID { get; set; }

        [JsonProperty("botname")]
        public string BotName { get; set; }

        [JsonProperty("verboseerrors")]
        public bool VerboseErrors { get; set; }

        [JsonProperty("8ballResponses")]
        public string[] EightBallResponses { get; set; }

        [JsonProperty("customReactions")]
        public Dictionary<string, List<string>> CustomReactions { get; set; }

        [JsonProperty("rotatePlayingStatuses")]
        public bool RotatePlaying { get; set; }

        [JsonProperty("playingStatuses")]
        public string[] PlayingStatuses { get; set; }

        [JsonProperty("googleAPIKey")]
        public string GoogleAPIKey { get; set; }

    }
}
