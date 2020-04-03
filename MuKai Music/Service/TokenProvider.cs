using Microsoft.IdentityModel.Tokens;
using MuKai_Music.Cache;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MuKai_Music.Service
{
    public static class TokenProvider
    {

        /// <summary>
        /// 颁发AccessToken 此token可用于访问接口
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static string CreateAccessToken(string userId)
        {
            DateTime expires = DateTime.UtcNow.AddMinutes(int.Parse(Startup.Configuration["Expires"]));
            Claim[] claims = new[]
            {
                new Claim (JwtRegisteredClaimNames.Exp,$"{expires.ToString()}"),
                new Claim ("type","ac"),
                new Claim("id", userId)
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Startup.Configuration["SecurityKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                audience: Startup.Configuration["Domain"],
            issuer: Startup.Configuration["Domain"],
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
        public static string CreateRefreshToken(string userId, string ua)
        {
            DateTime expires = DateTime.UtcNow.AddDays(int.Parse(Startup.Configuration["RefreshTime"])).ToUniversalTime();
            Claim[] claims = new[]
            {
                new Claim (JwtRegisteredClaimNames.Exp,$"{expires.ToString()}"),
                new Claim("id", userId)
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Startup.Configuration["SecurityKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                audience: Startup.Configuration["Domain"],
            issuer: Startup.Configuration["Domain"],
            claims: claims,
            expires: expires,
            signingCredentials: creds);
            string tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            TimeSpan expiry = expires - new DateTime(1970, 1, 1, 0, 0, 0);
            _ = RedisClient.RedisClientInstence.SetStringKeyAsync(GetTokenKey(userId, ua), tokenString, expiry);
            return tokenString;
        }

        public static string GetTokenKey(string userId, string ua)
        {
            return new StringBuilder(userId).Append("-").Append(ua).ToString();
        }
    }
}
