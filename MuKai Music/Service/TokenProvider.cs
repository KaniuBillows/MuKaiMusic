using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MuKai_Music.Cache;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MuKai_Music.Service
{
    public class TokenProvider
    {
        private readonly ICache cache;
        private readonly IConfiguration configuration;

        public TokenProvider(ICache cache, IConfiguration configuration)
        {
            this.cache = cache;
            this.configuration = configuration;
        }

        /// <summary>
        /// 颁发AccessToken 此token可用于访问接口
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public string CreateAccessToken(string userId)
        {
            DateTime expires = DateTime.UtcNow.AddMinutes(int.Parse(configuration.GetExpires()));
            Claim[] claims = new[]
            {
                new Claim (JwtRegisteredClaimNames.Exp,$"{expires}"),
                new Claim ("type","ac"),
                new Claim("id", userId)
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetSecurityKey()));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                audience: configuration.GetDomain(),
            issuer: configuration.GetDomain(),
            claims: claims,
            expires: expires,
            signingCredentials: creds);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        /// <summary>
        /// 颁发RefreshToken 此token可用于重新获取AccessToken
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="ua"></param>
        /// <returns></returns>
        public string CreateRefreshToken(string userId, string ua)
        {
            DateTime expires = DateTime.UtcNow.AddDays(int.Parse(configuration.GetRefreshTime())).ToUniversalTime();
            Claim[] claims = new[]
            {
                new Claim (JwtRegisteredClaimNames.Exp,$"{expires}"),
                new Claim("id", userId)
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetSecurityKey()));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                audience: configuration.GetDomain(),
            issuer: configuration.GetDomain(),
            claims: claims,
            expires: expires,
            signingCredentials: creds);
            string tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            TimeSpan expiry = expires - new DateTime(1970, 1, 1, 0, 0, 0);
            _ = this.cache.SetStringKeyAsync(GetTokenKey(userId, ua), tokenString, expiry);
            return tokenString;
        }

        public static string GetTokenKey(string userId, string ua)
        {
            return new StringBuilder(userId).Append("-").Append(ua).ToString();
        }
    }
}
