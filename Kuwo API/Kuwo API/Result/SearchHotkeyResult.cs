using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Kuwo_API.Result
{
    public class SearchHotkeyResult
    {
        [JsonPropertyName("code")] public int Code { get; set; }

        [JsonPropertyName("data")] public List<string> Data { get; set; }
    }
}