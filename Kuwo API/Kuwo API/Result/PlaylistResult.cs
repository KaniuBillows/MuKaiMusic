using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.Json.Serialization;
using DataAbstract;
using DataAbstract.Playlist;

namespace Kuwo_API.Result
{
    public class PlaylistResult
    {
        [JsonPropertyName("code")] public int Code { get; set; }
        [JsonPropertyName("msg")] public string Message { get; set; }
        [JsonPropertyName("data")] public DataRec Data { get; set; }

        public PlaylistInfo ToProcessedData()
        {
            PlaylistInfo result = new PlaylistInfo();
            if (Code != 200)
            {
                return result;
            }

            result.DataSource = DataSource.Kuwo;

            foreach (KuwoMusicInfo kuwoMusicInfo in Data.MusicList)
            {
                result.Musics.Add(new MusicInfo()
                {
                    Name = kuwoMusicInfo.Name,
                    Kw_Id = kuwoMusicInfo.Rid,
                    Artists = new Collection<Artist>()
                    {
                        new Artist()
                        {
                            Name = kuwoMusicInfo.Artist,
                            Id = kuwoMusicInfo.ArtistId
                        }
                    },
                    Duration = kuwoMusicInfo.Duration,
                    Album = new Album()
                    {
                        Name = kuwoMusicInfo.Album,
                        Id = kuwoMusicInfo.AlbumId,
                        PicUrl = kuwoMusicInfo.AlbumPic
                    },
                    DataSource = DataSource.Kuwo,
                });
            }

            result.MusicCount = Data.MusicList.Count;
            return result;
        }

        public class DataRec
        {
            [JsonPropertyName("id")] public long Id { get; set; }
            [JsonPropertyName("img300")] public string Image { get; set; }
            [JsonPropertyName("musicList")] public ICollection<KuwoMusicInfo> MusicList { get; set; }
        }
    }
}