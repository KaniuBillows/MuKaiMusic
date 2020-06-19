using System.Text.Json.Serialization;

namespace Kuwo_API.Result
{
    public class PicResult
    {
        [JsonPropertyName("code")]
        public int Code { get; set; }

        [JsonPropertyName("data")]
        public Data_ Data { get; set; }

        [JsonPropertyName("msg")]
        public string Message { get; set; }

        public class Data_
        {
            [JsonPropertyName("albumpic")]
            public string PicUrl { get; set; }
        }
    }
}
