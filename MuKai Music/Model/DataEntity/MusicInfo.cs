using MuKai_Music.Model.ResponseEntity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MuKai_Music.Model.DataEntity
{
    public class MusicInfo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int? Ne_Id { get; set; }

        public int? KuWo_Id { get; set; }

        /// <summary>
        /// 咪咕版权Id用于请求歌词，url等信息
        /// </summary>
        public string Migu_CopyrightId { get; set; }

        /// <summary>
        /// 咪咕歌曲Id，用于请求图片信息
        /// </summary>
        public string Migu_Id { get; set; }

        public string Name { get; set; }

        public string ArtistName { get; set; }

        public int? Ne_AlbumId { get; set; }

        public string AlbumName { get; set; }

        public string PicUrl { get; set; }

        public int? Ne_ArtistId { get; set; }

        public int? Duration { get; set; }

        [NotMapped]
        public DataSource DataSource { get; set; }
    }
}
