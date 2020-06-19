using DataAbstract;
using System;
using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

namespace Kuwo_API.Result
{
    public sealed class SearchResult
    {
        [JsonPropertyName("code")]
        public int Code { get; set; }

        /// <summary>
        /// 当前时间
        /// </summary>
        [JsonPropertyName("curTime")]
        public long CurTime { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("data")]
        public Data Data { get; set; }

        public MusicInfo[] ToProcessedData()
        {
            if (this.Code != 200)
            {
                return Array.Empty<MusicInfo>();
            }
            MusicInfo[] res = new MusicInfo[this.Data.List.Count];
            for (int i = 0; i < res.Length; i++)
            {
                res[i] = new MusicInfo()
                {
                    Name = this.Data.List[i].Name,
                    Kw_Id = this.Data.List[i].Rid,
                    Artists = new Collection<Artist>()
                    {
                        new Artist()
                        {
                           Name= this.Data.List[i].Artist,
                           Id =this.Data.List[i].ArtistId
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
        [JsonPropertyName("total")]
        public string Total { get; set; }

        [JsonPropertyName("list")]
        public Collection<Item> List { get; set; }
    }

    public class Item
    {
        [JsonPropertyName("musicid")]
        public string MusicId { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("songTimeMinutes")]
        public string SongTimeMinutes { get; set; }

        [JsonPropertyName("pic")]
        public string Pic { get; set; }

        [JsonPropertyName("rid")]
        public int Rid { get; set; }

        [JsonPropertyName("artist")]
        public string Artist { get; set; }

        [JsonPropertyName("artistid")]
        public int ArtistId { get; set; }

        [JsonPropertyName("duration")]
        public int Duration { get; set; }

        [JsonPropertyName("content_type")]
        public string ContentType { get; set; }

        [JsonPropertyName("releaseDate")]
        public string ReleaseDate { get; set; }

        [JsonPropertyName("album")]
        public string Album { get; set; }

        [JsonPropertyName("albumid")]
        public int AlbumId { get; set; }

        [JsonPropertyName("albumpic")]
        public string AlbumPic { get; set; }

    }
}
