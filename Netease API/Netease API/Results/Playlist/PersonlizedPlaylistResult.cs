using DataAbstract.Playlist;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json.Serialization;

namespace Netease_API.Results
{
    public class PersonlizedPlaylistResult
    {
        [JsonPropertyName("code")]
        public int Code { get; set; }

        [JsonPropertyName("result")]
        public Collection<PlaylistItem> Results { get; set; }

        public class PlaylistItem
        {
            [JsonPropertyName("id")]
            public long Id { get; set; }

            [JsonPropertyName("name")]
            public string Name { get; set; }

            [JsonPropertyName("picUrl")]
            public string PicUrl { get; set; }

            [JsonPropertyName("trackCount")]
            public int MusicCount { get; set; }
        }

        public PlaylistInfo[] ToProcessedData()
        {
            return this.Code != 200
                ? Array.Empty<PlaylistInfo>()
                : this.Results.Select(re =>
             {
                 return new PlaylistInfo()
                 {
                     MusicCount = re.MusicCount,
                     Id = re.Id,
                     Name = re.Name,
                     PicUrl = re.PicUrl,
                     DataSource = DataAbstract.DataSource.NetEase,
                     Musics = null
                 };
             }).ToArray();
        }
    }

}
