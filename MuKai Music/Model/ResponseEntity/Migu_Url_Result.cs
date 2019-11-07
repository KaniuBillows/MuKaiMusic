using System.Text.Json.Serialization;

namespace MuKai_Music.Model.ResponseEntity
{
    public class Migu_Url_Result
    {
        [JsonPropertyName("returnCode")]
        public string ReturnCode { get; set; }
        [JsonPropertyName("msg")]
        public string Message { get; set; }
        [JsonPropertyName("data")]
        public Data Data { get; set; }
    }


    public class Data
    {
        [JsonPropertyName("bqPlayInfo")]
        public Playinfo BqPlayInfo { get; set; }
        [JsonPropertyName("hqPlayInfo")]
        public Playinfo HqPlayInfo { get; set; }
        [JsonPropertyName("sqPlayInfo")]
        public Playinfo SqPlayInfo { get; set; }
    }

    public class Playinfo
    {
        [JsonPropertyName("playUrl")]
        public string PlayUrl { get; set; }
    }


}
