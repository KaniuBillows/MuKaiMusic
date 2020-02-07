using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MuKai_Music.Model.ResponseEntity
{
    public interface IResult<Type> : IResult
    {
        public Type Content { get; }
    }

    public interface IResult
    {
        public int Code { get; }

        public string Error { get; }
    }
}
