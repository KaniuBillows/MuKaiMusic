using System.ComponentModel.DataAnnotations;

namespace Mukai_Account.Model
{
    public class ChangePasswordModel
    {
        [RegularExpression
          (pattern: @"^[\w_-]{6,16}$",
          ErrorMessage = "密码为英文或数字组合，可以包含\"-\"与\"_\"，长度在6-16个字符之间")]
        [Required]
        public string OldPassword { get; set; }
        [RegularExpression
          (pattern: @"^[\w_-]{6,16}$",
          ErrorMessage = "密码为英文或数字组合，可以包含\"-\"与\"_\"，长度在6-16个字符之间")]
        [Required]
        public string NewPassword { get; set; }
    }
}