using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json.Serialization;
using DataAbstract;

namespace Netease_API.Results.Music
{
    public class FmResult
    {
        [JsonPropertyName("code")] public int Code { get; set; }
        [JsonPropertyName("data")] public Collection<Item> Data { get; set; }

        public List<MusicInfo> ToProcessedData()
        {
            var result = new List<MusicInfo>();
            if (Code != 200)
            {
                return result;
            }

            result.AddRange(Data.Select(item => new MusicInfo()
            {
                Name = item.Name,
                Ne_Id = item.Id,
                DataSource = DataSource.NetEase,
                Duration = item.Duration / 1000,
                Album = new DataAbstract.Album()
                    {Id = item.Album.Id, PicUrl = item.Album.PicUrl, Name = item.Album.Name},
                Artists = new Collection<DataAbstract.Artist>(item.Artists
                    .Select(artist => new DataAbstract.Artist() {Name = artist.Name, Id = artist.Id}).ToList())
            }));
            return result;
        }

        public class Item
        {
            [JsonPropertyName("name")] public string Name { get; set; }

            [JsonPropertyName("id")] public long Id { get; set; }

            [JsonPropertyName("duration")] public int Duration { get; set; }

            [JsonPropertyName("album")] public Album Album { get; set; }

            [JsonPropertyName("artists")] public Collection<Artist> Artists { get; set; }
        }

        public class Album
        {
            [JsonPropertyName("name")] public string Name { get; set; }
            [JsonPropertyName("id")] public long Id { get; set; }
            [JsonPropertyName("picUrl")] public string PicUrl { get; set; }
        }

        public class Artist
        {
            [JsonPropertyName("name")] public string Name { get; set; }
            [JsonPropertyName("id")] public long Id { get; set; }
        }
    }
}