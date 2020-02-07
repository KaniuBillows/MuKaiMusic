using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MuKai_Music.Model.ResponseEntity.MusicUrlResult
{
    public class MusicUrlInfo : ProcessedData
    {
        public MusicUrlInfo(DataSource searchType) : base(searchType)
        {
        }

        public string Url { get; set; }


    }
}
