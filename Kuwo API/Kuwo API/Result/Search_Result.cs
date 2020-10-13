using DataAbstract;
using System;
using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

namespace Kuwo_API.Result
{
    public sealed class SearchResult
    {
        [JsonPropertyName("code")] public int Code { get; set; }

        /// <summary>
        /// 当前时间
        /// </summary>
        [JsonPropertyName("curTime")]
        public long CurTime { get; set; }

        [JsonPropertyName("message")] public string Message { get; set; }

        [JsonPropertyName("data")] public Data Data { get; set; }

        public MusicInfo[] ToProcessedData()
        {
            if (this.Code != 200)
            {
                return Array.Empty<MusicInfo>();
            }

            var res = new MusicInfo[this.Data.List.Count];
            for (var i = 0; i < res.Length; i++)
            {
                res[i] = new MusicInfo()
                {
                    Name = this.Data.List[i].Name,
                    Kw_Id = this.Data.List[i].Rid,
                    Artists = new Collection<Artist>()
                    {
                        new Artist()
                        {
                            Name = this.Data.List[i].Artist,
                            Id = this.Data.List[i].ArtistId
                        }
                    },
                    Duration = this.Data.List[i].Duration,
                    Album = new Album()
                    {
                        Name = this.Data.List[i].Album,
                        PicUrl = this.Data.List[i].Pic,
                    },
                    DataSource = DataSource.Kuwo
                };
            }

            return res;
        }
    }

    public class Data
    {
        [JsonPropertyName("total")] public string Total { get; set; }
        [JsonPropertyName("list")] public Collection<KuwoMusicInfo> List { get; set; }
    }
}