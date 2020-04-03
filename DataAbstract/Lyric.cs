using System.Text.Json.Serialization;

namespace DataAbstract
{
    public class Lyric
    {
        [JsonPropertyName("time")]
        public float? Time { get; set; }

        [JsonPropertyName("text")]
        public string Text { get; set; }
    }
}
