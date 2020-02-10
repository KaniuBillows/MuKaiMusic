using MuKai_Music.Model.ResponseEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MuKai_Music.Model.RequestEntity.Music
{
    /// <summary>
    /// 用于请求音乐信息，例如URL，歌词等不同平台统一请求
    /// </summary>
    public class Music_Param
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
