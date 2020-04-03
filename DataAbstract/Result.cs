using System.Text.Json.Serialization;

namespace DataAbstract
{
    public class Result<T>
    {
        public Result(T content, int Code, string message)
        {
            this.Content = content;
            this.Code = Code;
            this.Message = message;
        }
        [JsonPropertyName("content")]
        public T Content { get; set; }

        [JsonPropertyName("code")]
        public int Code { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }
    }
}
