using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace DataAbstract.Account
{

    public class User
    {

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }


        [Required]
        [RegularExpression(pattern: @"^.{2,18}$", ErrorMessage = "昵称长度必须在2-18个字符之间")]
        public string NickName { get; set; }

        [Required]
        [RegularExpression
            (pattern: @"^[\w_-]{5,20}$",
            ErrorMessage = "用户名为英文与数字组合，可以包含\"-\"与\"_\"，长度在5-20个字符之间")]
        public string UserName { get; set; }

        public string AvatarUrl { get; set; }

        [RegularExpression
            (pattern: @"^[\w_-]{6,16}$",
            ErrorMessage = "密码为英文或数字组合，可以包含\"-\"与\"_\"，长度在8-16个字符之间")]
        [NotMapped]
        public string Password { get; set; }

        [RegularExpression(
            pattern: @"^[a-zA-Z0-9_-]+@[a-zA-Z0-9_-]+(\.[a-zA-Z0-9_-]+)+$",
            ErrorMessage = "请输入正确格式的邮箱地址，我们目前只支持英文和数字邮箱")]
        public string Email { get; set; }

        [RegularExpression(
            pattern: @"^1(3[0-9]|4[56789]|5[0-9]|6[6]|7[0-9]|8[0-9]|9[189])\d{8}$",
            ErrorMessage = "请输入正确的手机号")]
        public string PhoneNumber { get; set; }

        [JsonIgnore]
        public string PasswordHash { get; set; }

    }
}
