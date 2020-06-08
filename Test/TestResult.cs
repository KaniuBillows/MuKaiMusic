using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Test
{
    public class TestResult<T>
    {
        [JsonPropertyName("content")]
        public T Content { get; set; }

        [JsonPropertyName("code")]
        public int Code { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }
    }

    public class TestPageResult<T> : TestResult<List<T>>
    {
        [JsonPropertyName("totalCount")]
        public int TotalCount { get; set; }
    }
}
