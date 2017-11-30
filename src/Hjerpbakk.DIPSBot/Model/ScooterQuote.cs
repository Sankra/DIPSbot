using Newtonsoft.Json;

namespace Hjerpbakk.DIPSBot.Model
{
    public struct ScooterQuote
    {
        [JsonProperty("lyric")]
        public string Lyric { get; set; }

        [JsonProperty("song")]
        public string Song { get; set; }
    }
}
