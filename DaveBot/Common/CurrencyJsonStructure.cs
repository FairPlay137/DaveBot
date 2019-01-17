using System.Collections.Generic;
using Newtonsoft.Json;

namespace DaveBot.Common
{
    public struct CurrencyJsonStructure
    {
        [JsonProperty("prefix")]
        public string CommandPrefix { get; set; }

        [JsonProperty("currencyData")]
        public Dictionary<ulong, long> UserData { get; set; }
    }
}
