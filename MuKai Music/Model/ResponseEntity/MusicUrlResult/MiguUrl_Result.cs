using System.Text.Json.Serialization;

namespace MuKai_Music.Model.ResponseEntity.MusicUrlResult.Migu
{
    public class MiguUrl_Result : UnProcessedData<MusicUrlInfo>
    {
        [JsonPropertyName("returnCode")]
        public string ReturnCode { get; set; }
        [JsonPropertyName("msg")]
        public string Message { get; set; }
        [JsonPropertyName("data")]
        public Data Data { get; set; }

        public override MusicUrlInfo ToProcessedData() => throw new System.NotImplementedException();
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
