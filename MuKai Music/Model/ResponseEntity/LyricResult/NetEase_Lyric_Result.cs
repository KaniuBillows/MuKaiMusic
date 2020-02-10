using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MuKai_Music.Model.ResponseEntity.LyricResult
{
    public sealed class NetEase_Lyric_Result : UnProcessedData<Lyric[]>
    {
        [JsonPropertyName("code")]
        public int Code { get; set; }

        [JsonPropertyName("lrc")]
        public Lrc Lrc { get; set; }

        public override Lyric[] ToProcessedData()
        {
            if (this.Code != 200)
            {
                return Array.Empty<Lyric>();
            }
            else
            {
                Regex regex = new Regex("^\\[[0-9]{2}\\:[0-9]{2}\\.[0-9]{2,3}\\]");
                string[] lrcs = this.Lrc.Lyric.Split("\n");
                var lyrics = new List<Lyric>(lrcs.Length);
                foreach (string lrc in lrcs)
                {
                    if (regex.IsMatch(lrc))
                    {
                        var lyricItem = new Lyric();
                        // 时间信息字符串
                        string timeString = regex.Match(lrc).Value[1..^1];
                        float time = (int.Parse(timeString[0..2]) * 60)
                       + float.Parse(timeString[(timeString.IndexOf(':') + 1)..]);
                        lyricItem.Text = lrc.Replace(regex.Match(lrc).Value, "");
                        lyricItem.Time = time;
                        lyrics.Add(lyricItem);
                    }
                    else
                    {
                        lyrics.Add(new Lyric()
                        {
                            Text = lrc
                        });
                    }
                }
                return lyrics.ToArray();
            }
        }
    }

    public class Lrc
    {
        [JsonPropertyName("lyric")]
        public string Lyric { get; set; }
    }
}
