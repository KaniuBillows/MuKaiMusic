using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

namespace DataAbstract
{
    public class MusicInfo
    {
        /// <summary>
        /// 咪咕版权Id用于请求歌词，url等信息
        /// </summary>
        [JsonPropertyName("migu_CopyrightId")]
        public string Migu_CopyrightId { get; set; }

        /// <summary>
        /// 咪咕歌曲Id，用于请求图片信息
        /// </summary>
        [JsonPropertyName("migu_Id")]
        public int? Migu_Id { get; set; }

        [JsonPropertyName("ne_Id")]
        public long? Ne_Id { get; set; }

        [JsonPropertyName("kw_Id")]
        public int? Kw_Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("artists")]
        public Collection<Artist> Artists { get; set; }

        [JsonPropertyName("album")]
        public Album Album { get; set; }

        [JsonPropertyName("duration")]
        public int? Duration { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("dataSource")]
        public DataSource DataSource { get; set; }
    }

    public class Artist
    {
        [JsonPropertyName("id")]
        public long? Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }
    }

    public class Album
    {
        [JsonPropertyName("id")]
        public long? Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("picUrl")]
        public string PicUrl { get; set; }
    }
}

