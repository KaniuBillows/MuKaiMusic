using MuKai_Music.Model.DataEntity;
using MuKai_Music.Model.ResponseEntity.SearchResult;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

namespace MuKai_Music.Model.ResponseEntity.SearchResult.Migu
{
    public sealed class Migu_Search_Result : UnProcessedData<MusicInfo[]>
    {
        [JsonPropertyName("searchResult")]
        public Content SearchResult { get; set; }

        public override MusicInfo[] ToProcessedData()
        {
            if (this.SearchResult.Code != "000000")
            {
                return System.Array.Empty<MusicInfo>();
            }
            else
            {
                MusicInfo[] res = new MusicInfo[this.SearchResult.Object.SongList.Count];
                for (int i = 0; i < res.Length; i++)
                {
                    string name = this.SearchResult.Object.SongList[i].MusicName;
                    int index0 = name.IndexOf("<");
                    int index1 = name.IndexOf(">");
                    if (index1 != -1 && index1 != -1)
                    {
                        name = name.Replace(name.Substring(index0, index1 - index0 + 1), "");
                        int index2 = name.IndexOf("<");
                        int index3 = name.IndexOf(">");
                        name = name.Replace(name.Substring(index2, index3 - index2 + 1), "");
                    }
                    res[i] = new MusicInfo()
                    {
                        DataSource = DataSource.Migu,
                        Migu_CopyrightId = this.SearchResult.Object.SongList[i].CopyrightId,
                        Migu_Id = this.SearchResult.Object.SongList[i].MusicId,
                        Name = name,
                        ArtistName = this.SearchResult.Object.SongList[i].ArtistName,
                    };
                }

                return res;
            }
        }
    }

    public class Content
    {
        [JsonPropertyName("status")]
        public int Status { get; set; }
        [JsonPropertyName("error")]
        public object Error { get; set; }
        [JsonPropertyName("code")]
        public string Code { get; set; }
        [JsonPropertyName("desc")]
        public string Description { get; set; }
        [JsonPropertyName("object")]
        public Obj Object { get; set; }
    }

    public class Obj
    {
        [JsonPropertyName("songList")]
        public Collection<Song> SongList { get; set; }

        [JsonPropertyName("albumList")]
        public Collection<Album> AlbumList { get; set; }

        [JsonPropertyName("singerList")]
        public Collection<Artist> ArtistList { get; set; }
    }

    public class Song
    {
        [JsonPropertyName("fullSongCopyrightId")]
        public string CopyrightId { get; set; }

        [JsonPropertyName("musicId")]
        public string MusicId { get; set; }

        [JsonPropertyName("artistName")]
        public string ArtistName { get; set; }

        [JsonPropertyName("musicName")]
        public string MusicName { get; set; }

        [JsonPropertyName("highlightStr")]
        public string HilightString { get; set; }
    }
    public class Album
    {
        [JsonPropertyName("singerName")]
        public string SingerName { get; set; }

        [JsonPropertyName("albumName")]
        public string AlbumName { get; set; }

        [JsonPropertyName("albumId")]
        public string AlbumId { get; set; }

        [JsonPropertyName("highlightStr")]
        public string HilightString { get; set; }
    }
    public class Artist
    {
        [JsonPropertyName("artistId")]
        public string ArtistId { get; set; }

        [JsonPropertyName("artistName")]
        public string ArtistName { get; set; }

        [JsonPropertyName("highlightStr")]
        public string HilightString { get; set; }

    }

}
