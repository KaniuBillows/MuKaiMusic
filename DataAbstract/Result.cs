using System.Text.Json.Serialization;

namespace DataAbstract
{
    public class Result<T> where T : class
    {

        public static Result<T> SuccessReuslt(T content)
        {
            return new Result<T>(content, 200, "success");
        }

        public static Result<T> FailResult(string message)
        {
            return new Result<T>(null, 500, message);
        }

        public static Result<T> FailResult(string message, int code)
        {
            return new Result<T>(null, code, message);
        }

        public static Result<T> FailResult()
        {
            return new Result<T>(null, 500, "服务器错误");
        }

        protected Result(T content, int code, string message)
        {
            this.Content = content;
            this.Code = code;
            this.Message = message;
        }
        [JsonPropertyName("content")]
        public T Content { get; set; }

        [JsonPropertyName("code")]
        public int Code { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }
    }

    public class Result : Result<object>
    {
        protected Result(object content, int code, string message) : base(content, code, message)
        {
        }

        public static Result SuccessReuslt(string message)
        {
            return new Result(null, 200, message);
        }
        public new static Result FailResult(string message)
        {
            return new Result(null, 500, message);
        }

        public new static Result FailResult(string message, int code)
        {
            return new Result(null, code, message);
        }
    }
}
