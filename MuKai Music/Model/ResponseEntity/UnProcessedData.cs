using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MuKai_Music.Model.ResponseEntity
{
    public abstract class UnProcessedData<T>
    {
        public DataSource SourceType { get; set; }

        public abstract T ToProcessedData();
    }
}
