using System;
using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

namespace MuKai_Music.Model.ResponseEntity.SearchResult.NetEase
{
    public sealed class NetEase_Search_Result : UnProcessedData<SearchMusic[]>
    {
        [JsonPropertyName("code")]
        public int Code { get; set; }

        [JsonPropertyName("result")]
        public Result Content { get; set; }

        public override SearchMusic[] ToProcessedData()
        {
            if (this.Code != 200)
            {
                return Array.Empty<SearchMusic>();
            }
            SearchMusic[] res = new SearchMusic[this.Content.Songs.Count];
            for (int i = 0; i < res.Length; i++)
            {
                res[i] = new SearchMusic(DataSource.NetEase)
                {
                    Name = this.Content.Songs[i].Name,
                    NetEaseId = this.Content.Songs[i].Id,
                    Aritst = new SearchResult.Artist(DataSource.NetEase)
                    {
                        NetEaseId = this.Content.Songs[i].Artists[0].Id,
                        Name = this.Content.Songs[i].Artists[0].Name
                    },
                    Album = new SearchResult.Album(DataSource.NetEase)
                    {
                        NetEaseId = this.Content.Songs[i].Album.Id,
                        Name = this.Content.Songs[i].Album.Name
                    }
                };
            }
            return res;
        }

    }

    public class Result
    {
        [JsonPropertyName("songs")]
        public Collection<Song> Songs { get; set; }

        [JsonPropertyName("songCount")]
        public int SongCount { get; set; }
    }

    public class Song
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("artists")]
        public Collection<Artist> Artists { get; set; }

        [JsonPropertyName("album")]
        public Album Album { get; set; }
    }

    public class Artist
    {

        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("picUrl")]
        public string PicUrl { get; set; }
    }

    public class Album
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("artist")]
        public Artist Artist { get; set; }

        [JsonPropertyName("publishTime")]
        public long PublishTime { get; set; }

        [JsonPropertyName("picId")]
        public long PicId { get; set; }
    }
}
