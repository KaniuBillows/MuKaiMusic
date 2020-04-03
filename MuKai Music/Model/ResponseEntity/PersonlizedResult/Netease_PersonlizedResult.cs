/*using MuKai_Music.Model.DataEntity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MuKai_Music.Model.ResponseEntity.PersonlizedResult
{
    public class Netease_PersonlizedResult : UnProcessedData<MusicInfo[]>
    {
        [JsonPropertyName("code")]
        public int Code { get; set; }

        [JsonPropertyName("result")]
        public Collection<Result> Result { get; set; }

        public override MusicInfo[] ToProcessedData()
        {
            if (this.Code != 200) return Array.Empty<MusicInfo>();
            MusicInfo[] res = new MusicInfo[this.Result.Count];
            for (int i = 0; i < res.Length; i++)
            {
                res[i] = new MusicInfo()
                {
                    Ne_Id = this.Result[i].Id,
                    ArtistName = this.Result[i].Song.Artists[0].Name,
                    PicUrl = this.Result[i].PicUrl + "?param=240y240",
                    Name = this.Result[i].Name,
                    Duration = this.Result[i].Song.Duration / 1000,
                    Ne_ArtistId = this.Result[i].Song.Artists[0].Id,
                    Ne_AlbumId = this.Result[i].Song.Album.Id,
                    AlbumName = this.Result[i].Song.Album.Name,
                    DataSource = DataSource.NetEase
                };
            }
            return res;
        }
    }
    public class Result
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("picUrl")]
        public string PicUrl { get; set; }

        [JsonPropertyName("song")]
        public Song Song { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
    public class Song
    {
        [JsonPropertyName("duration")]
        public int Duration { get; set; }

        [JsonPropertyName("artists")]
        public Collection<Artist> Artists { get; set; }

        [JsonPropertyName("album")]
        public Album Album { get; set; }
    }
    public class Artist
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("id")]
        public int Id { get; set; }
    }

    public class Album
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("id")]
        public int Id { get; set; }
    }
}
*/