using System.Collections.ObjectModel;
using System.Text.Json.Serialization;
using MongoDB.Bson.Serialization.Attributes;

namespace DataAbstract
{
    public class MusicInfo
    {
        /// <summary>
        /// 咪咕版权Id用于请求歌词，url等信息
        /// </summary>
        [JsonPropertyName("migu_CopyrightId")]
        [BsonElement("migu_copyrightId")]
        public string Migu_CopyrightId { get; set; }

        /// <summary>
        /// 咪咕歌曲Id，用于请求图片信息
        /// </summary>
        [JsonPropertyName("migu_Id")]
        [BsonElement("migu_id")]
        public int? Migu_Id { get; set; }

        [JsonPropertyName("ne_Id")]
        [BsonElement("ne_id")]
        public long? Ne_Id { get; set; }

        [JsonPropertyName("kw_Id")]
        [BsonElement("kw_id")]
        public int? Kw_Id { get; set; }

        [JsonPropertyName("name")]
        [BsonElement("name")]
        public string Name { get; set; }

        [JsonPropertyName("artists")]
        [BsonElement("artists")]
        public Collection<Artist> Artists { get; set; }

        [JsonPropertyName("album")]
        [BsonElement("album")]
        public Album Album { get; set; }

        [JsonPropertyName("duration")]
        [BsonElement("duration")]
        public int? Duration { get; set; }

        [JsonPropertyName("url")]
        [BsonElement("url")]
        public string Url { get; set; }

        [JsonPropertyName("dataSource")]
        [BsonElement("dataSource")]
        public DataSource DataSource { get; set; }

        /// <summary>
        /// 格式:0_XXX 下划线前方代表DataSource,后方代表Id
        /// </summary>
        [BsonElement("sid")]
        [JsonPropertyName("sid")]
        public string Sid { get; set; }
    }

    public class Artist
    {
        [JsonPropertyName("id")]
        [BsonElement("id")]
        public long? Id { get; set; }

        [JsonPropertyName("name")]
        [BsonElement("name")]
        public string Name { get; set; }
    }

    public class Album
    {
        [JsonPropertyName("id")]
        [BsonElement("id")]
        public long? Id { get; set; }

        [JsonPropertyName("name")]
        [BsonElement("name")]
        public string Name { get; set; }

        [JsonPropertyName("picUrl")]
        [BsonElement("picUrl")]
        public string PicUrl { get; set; }
    }

    public static class MusicInfoExtion
    {
        public static string GetSid(this MusicInfo musicInfo)
        {
            return musicInfo.DataSource switch
            {
                DataSource.NetEase => musicInfo.DataSource.ToString() + "_" + musicInfo.Ne_Id,
                DataSource.Migu => musicInfo.DataSource.ToString() + "_" + musicInfo.Migu_CopyrightId,
                DataSource.Kuwo => musicInfo.DataSource.ToString() + "_" + musicInfo.Kw_Id,
                _ => null,
            };
        }
    }
}