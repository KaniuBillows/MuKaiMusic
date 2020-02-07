using MuKai_Music.Model.ResponseEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MuKai_Music.Model.RequestEntity.Music
{
    public class MusicUrl_Param
    {
        [Required]
        [JsonPropertyName("dataSource")]
        public DataSource DataSource { get; set; }

        [JsonPropertyName("miguId")]
        public string MiguId { get; set; }

        [JsonPropertyName("netEaseId")]
        public int? NeteaseId { get; set; }

        [JsonPropertyName("kuwoId")]
        public int? KuwoId { get; set; }
    }
}
