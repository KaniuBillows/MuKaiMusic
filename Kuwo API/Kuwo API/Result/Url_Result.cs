using System.Collections.Generic;
using DataAbstract;
using System.Text.Json.Serialization;

namespace Kuwo_API.Result
{
    public sealed class KuwoUrl_Result
    {
        [JsonPropertyName("code")] public int Code { get; set; }

        [JsonPropertyName("msg")] public string Message { get; set; }

        [JsonPropertyName("data")] public Dictionary<string, string> Data { get; set; }

        public UrlInfo ToProcessedData()
        {
            var res = new UrlInfo(DataSource.Kuwo);

            if (Data.TryGetValue("url", out var val))
            {
                res.Url = val;
            }
            return res;
        }
    }
}