using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

namespace Netease_API.Results
{
    public sealed class NetEaseUrl_Result
    {
        [JsonPropertyName("data")]
        public Collection<Url_Info> Data { get; set; }

        [JsonPropertyName("code")]
        public int Code { get; set; }

        public Dictionary<long, string> ToProcessedData()
        {
            Dictionary<long, string> res = new Dictionary<long, string>();
            if (this.Code != 200) return res;
            foreach (Url_Info data in this.Data)
            {
                res.Add(data.Id, data.Url);
            }
            return res;
        }

    }
    public class Url_Info
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }

        /// <summary>
        /// 码率
        /// </summary>
        [JsonPropertyName("br")]
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
