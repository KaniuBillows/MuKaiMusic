using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MuKai_Music.Model.ResponseEntity.LyricResult
{
    public class Lyric
    {
        [JsonPropertyName("time")]
        public float? Time { get; set; }

        [JsonPropertyName("text")]
        public string Text { get; set; }
    }


}
