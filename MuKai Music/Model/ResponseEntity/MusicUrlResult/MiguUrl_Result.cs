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

        public override MusicUrlInfo ToProcessedData()
        {
            return this.ReturnCode != "000000"
                ? new MusicUrlInfo(DataSource.Migu)
                : new MusicUrlInfo(DataSource.Migu)
                {
                    Url = this.Data.SqPlayInfo != null
                    ? this.Data.SqPlayInfo.PlayUrl
                    : this.Data.HqPlayInfo != null
                    ? this.Data.HqPlayInfo.PlayUrl
                    : this.Data.BqPlayInfo?.PlayUrl
                };

        }
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
