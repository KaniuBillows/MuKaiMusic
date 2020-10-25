using DataAbstract;
using System.Text.Json.Serialization;

namespace Kuwo_API.Result
{
    public sealed class KuwoUrl_Result
    {
        [JsonPropertyName("code")]
        public int Code { get; set; }

        [JsonPropertyName("msg")]
        public string Message { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }

        public UrlInfo ToProcessedData() =>
            this.Code != 200 ? new UrlInfo(DataSource.Kuwo) :
                new UrlInfo(DataSource.Kuwo)
                {
                    Url = this.Url.Replace("http","https")
                };
    }
}
