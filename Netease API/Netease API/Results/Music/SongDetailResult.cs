using DataAbstract;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json.Serialization;

namespace Netease_API.Results.Music
{
    /// <summary>
    /// 获取歌曲详情
    /// </summary>
    public class SongDetailResult
    {
        [JsonPropertyName("songs")]
        public Collection<Song> Songs { get; set; }

        [JsonPropertyName("code")]
        public int Code { get; set; }

        public class Song
        {
            [JsonPropertyName("id")]
            public long Id { get; set; }

            [JsonPropertyName("name")]
            public string Name { get; set; }

            [JsonPropertyName("ar")]
            public Collection<Ar> Artists { get; set; }

            [JsonPropertyName("al")]
            public Al Album { get; set; }

            [JsonPropertyName("duration")]
            public int? Duration { get; set; }
        }

        public Dictionary<long, string> GetIdPicInfo()
        {
            Dictionary<long, string> res = new Dictionary<long, string>();
            foreach (Song song in Songs)
            {
                res.Add(song.Id, song.Album.PicUrl);
            }
            return res;
        }

        public List<MusicInfo> ToProcessedData()
        {
            return this.Code != 200
                ? new List<MusicInfo>()
                : this.Songs.Select(song =>
             {
                 return new MusicInfo()
                 {
                     Ne_Id = song.Id,
                     DataSource = DataSource.NetEase,
                     Name = song.Name,
                     Album = new DataAbstract.Album()
                     {
                         Id = song.Album.Id,
                         Name = song.Album.Name,
                         PicUrl = song.Album.PicUrl
                     },
                     Artists = new Collection<DataAbstract.Artist>(
                         song.Artists.Select(ar => new DataAbstract.Artist
                         {
                             Name = ar.Name,
                             Id = ar.Id
                         }).ToList()),
                     Duration = song.Duration,
                 };
             }).ToList();
        }
    }

    public class Al
    {
        [JsonPropertyName("picUrl")]
        public string PicUrl { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("id")]
        public long Id { get; set; }
    }

    public class Ar
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("id")]
        public long Id { get; set; }
    }

}
