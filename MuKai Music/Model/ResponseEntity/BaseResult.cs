using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MuKai_Music.Model.ResponseEntity
{
    public class BaseResult<T> : IResult<T>
    {
        public BaseResult(T Content, int Code, string Error)
        {
            this.Code = Code;
            this.Content = Content;
            this.Error = Error;
        }

        public T Content { get; }

        public int Code { get; }

        public string Error { get; }
    }
}
