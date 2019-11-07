using System.Text.Json.Serialization;

namespace MuKai_Music.Model.ResponseEntity
{
    public class Migu_Search_Result
    {
        [JsonPropertyName("searchResult")]
        public Searchresult SearchResult { get; set; }

    }

    public class Searchresult
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
        public List List { get; set; }
    }

    public class List
    {
        [JsonPropertyName("songList")]
        public Songlist[] SongList { get; set; }
    }

    public class Songlist
    {
        [JsonPropertyName("fullSongCopyrightId")]
        public string CopyrightId { get; set; }

    }

}
