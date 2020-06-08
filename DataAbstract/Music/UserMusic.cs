using System.Collections.Generic;
using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DataAbstract.Music
{
    public class UserMusic
    {
        [BsonId]
        [BsonRepresentation(BsonType.Int64)]
        [BsonElement("_id")]
        [JsonPropertyName("userId")]
        public long UserId { get; set; }

        [BsonElement("playlists")]
        [JsonPropertyName("userPlaylists")]
        public List<UserPlaylist> UserPlaylists { get; set; }

        [BsonElement("subscribePlaylists")]
        [JsonPropertyName("subscribePlaylists")]
        [BsonRepresentation(BsonType.ObjectId)]
        public List<string> SubscribePlaylists { get; set; }

        [BsonElement("likes")]
        [JsonPropertyName("likes")]
        public List<MusicInfo> Likes { get; set; }

    }
}
