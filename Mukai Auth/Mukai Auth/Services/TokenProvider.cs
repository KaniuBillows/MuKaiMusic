using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Mukai_Auth.Service
{
    public class TokenProvider
    {
        private readonly IConfiguration configuration;

        public TokenProvider(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        /// <summary>
        /// 颁发AccessToken 此token可用于访问接口
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public string CreateAccessToken(string userId)
        {
            DateTime expires = DateTime.UtcNow.AddDays(int.Parse(configuration["Expires"]));
            Claim[] claims = new[]
            {
                new Claim (JwtRegisteredClaimNames.Exp,$"{expires}"),
                new Claim ("type","ac"),
                new Claim("id", userId)
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Secret"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                audience: configuration["Domain"],
            issuer: configuration["Domain"],
            claims: claims,
            expires: expires,
            signingCredentials: creds);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public static string GetTokenKey(string userId, string ua)
        {
            return new StringBuilder(userId).Append("-").Append(ua).ToString();
        }
    }
}
