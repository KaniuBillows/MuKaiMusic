using System.Text.Json.Serialization;
using Chloe.Annotations;
using ColumnAttribute = Chloe.Annotations.ColumnAttribute;

namespace DataAbstract.Account
{

    public class User
    {

        [Column(IsPrimaryKey = true)]
        [AutoIncrement]
        public long? Id { get; set; }

        public string NickName { get; set; }

        public string AvatarUrl { get; set; }

        public string Email { get; set; }

        public string NormalizedEmail { get; set; }

        public string PhoneNumber { get; set; }

        [JsonIgnore]
        public string PasswordHash { get; set; }
    }
}
