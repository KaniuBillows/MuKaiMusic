using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataAbstract
{
    [Serializable]
    public class PageResult<T> : Result<List<T>>
    {
        protected PageResult(List<T> content, int Code, string message) : base(content, Code, message)
        {

        }

        public static PageResult<T> SuccessResult(List<T> content, int total)
        {
            if (content is null) throw new ArgumentNullException(nameof(content) + " cannot be null! ");
            return new PageResult<T>(content, 200, "success")
            {
                TotalCount = total
            };
        }

        public new static PageResult<T> FailResult(string message)
        {
            return new PageResult<T>(null, 500, message)
            {
                TotalCount = 0
            };
        }

        public new static PageResult<T> FailResult(string message, int code)
        {
            return new PageResult<T>(null, code, message)
            {
                TotalCount = 0
            };
        }

        [JsonPropertyName("totalCount")]
        public int TotalCount { get; set; }
    }
}
