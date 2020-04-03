using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MuKai_Music.Model.DataEntity
{
    public enum LoginType
    {
        CellPhone,
        Email
    }

    public class UserInfo : IdentityUser<int>
    {

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public override int Id { get; set; }

        public int? Ne_Id { get; set; }

        public LoginType LoginType { get; set; }

        [Required]
        [StringLength(maximumLength: 30, MinimumLength = 2, ErrorMessage = "昵称长度必须在2-30个字符之间")]
        public string NcikName { get; set; }

        [Required]
        [RegularExpression
            (pattern: @"^[\w_-]{6,20}$",
            ErrorMessage = "用户名为英文与数字组合，可以包含\"-\"与\"_\"，长度在6-20个字符之间")]
        public override string UserName { get; set; }

        public string AvatarUrl { get; set; }

        [RegularExpression
            (pattern: @"^[\w_-]{6,16}$",
            ErrorMessage = "密码为英文或数字组合，可以包含\"-\"与\"_\"，长度在8-16个字符之间")]
        [NotMapped]
        public string Password { get; set; }

        [RegularExpression(
            pattern: @"^[a-zA-Z0-9_-]+@[a-zA-Z0-9_-]+(\.[a-zA-Z0-9_-]+)+$",
            ErrorMessage = "请输入正确格式的邮箱地址，我们目前只支持英文和数字邮箱")]
        [ProtectedPersonalData]
        public override string Email { get; set; }

        [RegularExpression(
            pattern: @"^1(3[0-9]|4[56789]|5[0-9]|6[6]|7[0-9]|8[0-9]|9[189])\d{8}$",
            ErrorMessage = "请输入正确的手机号"
            )]
        [ProtectedPersonalData]
        public override string PhoneNumber { get; set; }

        [JsonIgnore]
        public override string PasswordHash { get => base.PasswordHash; set => base.PasswordHash = value; }

        [JsonIgnore]
        public override string SecurityStamp { get => base.SecurityStamp; set => base.SecurityStamp = value; }

        [JsonIgnore]
        public override string NormalizedUserName { get => base.NormalizedUserName; set => base.NormalizedUserName = value; }

        [JsonIgnore]
        public override string NormalizedEmail { get => base.NormalizedEmail; set => base.NormalizedEmail = value; }

        [JsonIgnore]
        public override string ConcurrencyStamp { get => base.ConcurrencyStamp; set => base.ConcurrencyStamp = value; }
        /// <summary>
        /// 网易云手机号
        /// </summary>
        [JsonIgnore]
        public string Ne_Cellphone { get; set; }

        /// <summary>
        /// 网易云密码
        /// </summary>
        [JsonIgnore]
        public string Ne_Password { get; set; }
    }
}
