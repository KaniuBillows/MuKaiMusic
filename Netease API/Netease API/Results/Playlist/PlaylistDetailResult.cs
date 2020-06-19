using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

namespace Netease_API.Results.Playlist
{
    public class PlaylistDetailResult
    {
        [JsonPropertyName("code")]
        public int Code { get; set; }



        [JsonPropertyName("playlist")]
        public Element Playlist { get; set; }

        public class Element
        {
            [JsonPropertyName("name")]
            public string Name { get; set; }

            [JsonPropertyName("id")]
            public long Id { get; set; }

            [JsonPropertyName("trackIds")]
            public Collection<IdInfo> SongIds { get; set; }

            [JsonPropertyName("trackCount")]
            public int SongCount { get; set; }

            [JsonPropertyName("coverImgUrl")]
            public string PicUrl { get; set; }

        }
        public class IdInfo
        {
            [JsonPropertyName("id")]
            public long Id { get; set; }
        }
    }
}
