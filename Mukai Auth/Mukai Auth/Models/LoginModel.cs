using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Mukai_Auth.Models
{
    public class LoginModel
    {
        [JsonPropertyName("loginName")]
        public string LoginName { get; set; }

        [JsonPropertyName("passwordHashed")]
        public string PasswordHashed { get; set; }
    }
}
