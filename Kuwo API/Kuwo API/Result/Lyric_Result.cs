using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json.Serialization;

namespace Kuwo_API.Result.Lyric
{
    public sealed class LyricResult
    {
        [JsonPropertyName("data")]
        public Data Data { get; set; }

        [JsonPropertyName("msg")]
        public string Message { get; set; }

        [JsonPropertyName("status")]
        public int Status { get; set; }

        public DataAbstract.Lyric[] ToProcessedData()
        {
            return this.Status != 200
                ? Array.Empty<DataAbstract.Lyric>()
                : this.Data.Lyrics.Select((lrc) =>
                       new DataAbstract.Lyric()
                       {
                           Text = lrc.Text,
                           Time = float.Parse(lrc.Time)
                       }).ToArray();
        }

    }

    public class Data
    {
        [JsonPropertyName("lrclist")]
        public Collection<LyricItem> Lyrics { get; set; }
    }

    public class LyricItem
    {
        [JsonPropertyName("time")]
        public string Time { get; set; }

        [JsonPropertyName("lineLyric")]
        public string Text { get; set; }
    }
}
