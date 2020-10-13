using System.Text.Json.Serialization;

namespace Kuwo_API.Result
{
    public class KuwoMusicInfo
    {
        [JsonPropertyName("musicrid")] public string MusicId { get; set; }

        [JsonPropertyName("name")] public string Name { get; set; }

        [JsonPropertyName("songTimeMinutes")] public string SongTimeMinutes { get; set; }

        [JsonPropertyName("pic")] public string Pic { get; set; }

        [JsonPropertyName("rid")] public int Rid { get; set; }

        [JsonPropertyName("artist")] public string Artist { get; set; }

        [JsonPropertyName("artistid")] public int ArtistId { get; set; }

        [JsonPropertyName("duration")] public int Duration { get; set; }

        [JsonPropertyName("content_type")] public string ContentType { get; set; }

        [JsonPropertyName("releaseDate")] public string ReleaseDate { get; set; }

        [JsonPropertyName("album")] public string Album { get; set; }

        [JsonPropertyName("albumid")] public int AlbumId { get; set; }

        [JsonPropertyName("albumpic")] public string AlbumPic { get; set; }
    }
}