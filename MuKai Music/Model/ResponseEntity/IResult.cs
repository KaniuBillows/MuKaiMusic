using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MuKai_Music.Model.ResponseEntity
{
    public interface IResult<Type>
    {
        public Type Content { get; }

        public int Code { get; }

        public string Error { get; }
    }
}
