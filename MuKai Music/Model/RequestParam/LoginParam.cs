using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MuKai_Music.Model.RequestParam
{
    public class LoginParam
    {
        [JsonPropertyName("userName")]
        [Required]
        public string UserName { get; set; }

        [JsonPropertyName("password")]
        [Required]
        public string Password { get; set; }
    }
}
