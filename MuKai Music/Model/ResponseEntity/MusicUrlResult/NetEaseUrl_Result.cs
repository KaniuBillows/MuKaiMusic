using System.Text.Json.Serialization;

namespace MuKai_Music.Model.ResponseEntity.MusicUrlResult.NetEase
{
    public sealed class NetEaseUrl_Result : UnProcessedData<MusicUrlInfo>
    {
        [JsonPropertyName("data")]
        public Url_Info[] Data { get; set; }

        [JsonPropertyName("code")]
        public int Code { get; set; }

        public override MusicUrlInfo ToProcessedData() =>
            this.Code != 200 ? new MusicUrlInfo(DataSource.NetEase) :
                new MusicUrlInfo(DataSource.NetEase)
                {
                    NetEaseId = this.Data[0].Id,
                    Url = this.Data[0].Url
                };
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
