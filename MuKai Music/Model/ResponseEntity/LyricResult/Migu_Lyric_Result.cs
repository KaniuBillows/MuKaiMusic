using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MuKai_Music.Model.ResponseEntity.LyricResult
{
    public class Migu_Lyric_Result : UnProcessedData<Lyric[]>
    {
        [JsonPropertyName("returnCode")]
        public string ReturnCode { get; set; }

        [JsonPropertyName("lyric")]
        public string Lyric { get; set; }

        public override Lyric[] ToProcessedData()
        {
            if (this.ReturnCode != "000000")
            {
                return Array.Empty<Lyric>();
            }
            else
            {
                var regex = new Regex("^\\[[0-9]{2}\\:[0-9]{2}\\.[0-9]{2}\\]");
                string[] strs = this.Lyric.Split("\n");
                var lyrics = new List<Lyric>(strs.Length);
                foreach (string str in strs)
                {
                    if (regex.IsMatch(str))
                    {
                        var lyricItem = new Lyric();
                        // 时间信息字符串
                        string timeString = regex.Match(str).Value[1..^1];
                        float time = (int.Parse(timeString[0..2]) * 60)
                       + float.Parse(timeString[(timeString.IndexOf(':') + 1)..]);
                        lyricItem.Text = str.Replace(regex.Match(str).Value, "");
                        lyricItem.Time = time;
                        lyrics.Add(lyricItem);
                    }
                    else
                    {
                        lyrics.Add(new Lyric()
                        {
                            Text = str
                        });
                    }
                }
                return lyrics.ToArray();
            }

        }
    }
}
