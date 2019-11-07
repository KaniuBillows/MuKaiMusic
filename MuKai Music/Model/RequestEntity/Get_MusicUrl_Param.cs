using System.Text.Json.Serialization;

namespace MuKai_Music.Model.RequestEntity
{
    public class Get_MusicUrl_Param
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("br")]
        public int Br { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("artist")]
        public string Artist { get; set; }
    }
}
