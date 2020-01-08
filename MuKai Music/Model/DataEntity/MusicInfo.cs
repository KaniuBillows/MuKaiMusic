using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MuKai_Music.Model.DataEntity
{
    public class MusicInfo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int Ne_Id { get; set; }

        public int? KuWo_Id { get; set; }

        public int? Migu_Id { get; set; }

        public string Name { get; set; }

        public string ArtistName { get; set; }

        public int? Ne_ArtistId { get; set; }

        public string Migu_Url { get; set; }
    }
}
