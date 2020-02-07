using System.Text.Json.Serialization;

namespace MuKai_Music.Model.ResponseEntity.MusicUrlResult.Kuwo
{
    public sealed class KuwoUrl_Result : UnProcessedData<MusicUrlInfo>
    {
        [JsonPropertyName("code")]
        public int Code { get; set; }

        [JsonPropertyName("msg")]
        public string Message { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }

        public override MusicUrlInfo ToProcessedData() =>
            this.Code != 200 ? new MusicUrlInfo(DataSource.Kuwo) :
                new MusicUrlInfo(DataSource.Kuwo)
                {
                    Url = this.Url
                };
    }
}
