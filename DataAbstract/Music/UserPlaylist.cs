using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataAbstract.Music
{
    /// <summary>
    /// 用户创建的歌单
    /// </summary>
    public class UserPlaylist
    {
        [BsonElement("_id")]
        [JsonPropertyName("id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("userId")]
        [JsonPropertyName("userId")]
        public long? UserId { get; set; }

        [BsonElement("name")]
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [BsonElement("picUrl")]
        [JsonPropertyName("picUrl")]
        public string PicUrl { get; set; }

        [BsonElement("public")]
        [JsonPropertyName("public")]
        public bool? Public { get; set; }

        [BsonElement("createTime")]
        [JsonPropertyName("createTime")]
        public DateTime? CreateTime { get; set; }

        [BsonElement("playedCount")]
        [JsonPropertyName("playedCount")]
        public long? PlayedCount { get; set; }

        [BsonElement("tracks")]
        [JsonPropertyName("tracks")]
        public List<MusicInfo> Tracks { get; set; }
    }
}
