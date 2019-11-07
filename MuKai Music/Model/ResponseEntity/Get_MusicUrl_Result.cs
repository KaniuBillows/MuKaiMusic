using System.Text.Json.Serialization;

namespace MuKai_Music.Model.ResultEntity
{
    public class Get_MusicUrl_Result
    {
        [JsonPropertyName("data")]
        public Url_Info[] Data { get; set; }

    }
    public class Url_Info
    {
        [JsonPropertyName("Id")]
        public int Id { get; set; }
        [JsonPropertyName("url")]
        public string Url { get; set; }
        /// <summary>
        /// 码率
        /// </summary>
        [JsonPropertyName("Br")]
        public int Br { get; set; }
        /// <summary>
        /// 大小
        /// </summary>
        [JsonPropertyName("size")]
        public int Size { get; set; }
        [JsonPropertyName("code")]
        public int Code { get; set; }
        /// <summary>
        /// 格式
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; }

    }
}
