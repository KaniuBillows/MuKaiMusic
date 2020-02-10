﻿using MuKai_Music.Model.DataEntity;
using System;
using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

namespace MuKai_Music.Model.ResponseEntity.SearchResult.NetEase
{
    public sealed class NetEase_Search_Result : UnProcessedData<MusicInfo[]>
    {
        [JsonPropertyName("code")]
        public int Code { get; set; }

        [JsonPropertyName("result")]
        public Result Content { get; set; }

        public override MusicInfo[] ToProcessedData()
        {
            if (this.Code != 200)
            {
                return Array.Empty<MusicInfo>();
            }
            MusicInfo[] res = new MusicInfo[this.Content.Songs.Count];
            for (int i = 0; i < res.Length; i++)
            {
                res[i] = new MusicInfo()
                {
                    DataSource = DataSource.NetEase,
                    Name = this.Content.Songs[i].Name,
                    Ne_Id = this.Content.Songs[i].Id,
                    ArtistName = this.Content.Songs[i].Artists[0].Name,
                    Ne_ArtistId = this.Content.Songs[i].Artists[0].Id,
                    AlbumName = this.Content.Songs[i].Album.Name,
                    Ne_AlbumId = this.Content.Songs[i].Album.Id,
                    Duration = this.Content.Songs[i].Duration/1000
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
        [JsonPropertyName("duration")]
        public int Duration { get; set; }

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
