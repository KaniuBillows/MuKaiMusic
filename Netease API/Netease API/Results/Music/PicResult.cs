using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

namespace Netease_API.Results
{
    public class PicResult
    {

        [JsonPropertyName("songs")]
        public Collection<Song> Songs { get; set; }

        [JsonPropertyName("code")]
        public int Code { get; set; }

        public class Song
        {
            [JsonPropertyName("al")]
            public Album Album { get; set; }
        }

        public class Album
        {
            [JsonPropertyName("picUrl")]
            public string PicUrl { get; set; }
        }
    }

}
