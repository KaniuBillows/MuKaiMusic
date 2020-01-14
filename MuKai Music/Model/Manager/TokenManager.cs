using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MuKai_Music.Model.DataEntity;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MuKai_Music.Model.Manager
{
    public class TokenManager
    {
        /// <summary>
        /// 颁发AccessToken,用于Api访问
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static string GetAccessToken(UserInfo user)
        {
            return GetAccessToken(user.UserName);
        }

        public static string GetAccessToken(string username)
        {
            var claims = new[]
              {
                    new Claim(JwtRegisteredClaimNames.Nbf,$"{new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds()}") ,
                    new Claim (JwtRegisteredClaimNames.Exp,$"{new DateTimeOffset(DateTime.Now.AddMinutes(Startup.Config.GetValue<int>("Expires"))).ToUnixTimeSeconds()}"),
                    new Claim(ClaimTypes.Name, username)
                };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Startup.Config.GetValue<string>("SecurityKey")));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddMinutes(Startup.Config.GetValue<int>("Expires"));

            var token = new JwtSecurityToken(
                issuer: Startup.Config.GetValue<string>("Domain"),
                audience: Startup.Config.GetValue<string>("Domain"),
                claims: claims,
                expires: expires,
                signingCredentials: creds);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// 颁发RefreshToken 用于刷新AccessToken
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static string GetRefreshToken(UserInfo user)
        {
            var claims = new[]
            {
                  new Claim(JwtRegisteredClaimNames.Nbf,$"{new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds()}") ,
                    new Claim (JwtRegisteredClaimNames.Exp,$"{new DateTimeOffset(DateTime.Now.AddDays(30)).ToUnixTimeSeconds()}"),
                    new Claim(ClaimTypes.Name, user.UserName)
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Startup.Config.GetValue<string>("SecurityKey")));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: Startup.Config.GetValue<string>("Domain"),
                audience: Startup.Config.GetValue<string>("Domain"),
                claims: claims,
                expires: DateTime.Now.AddMinutes(Startup.Config.GetValue<int>("RefreshTime")),
                signingCredentials: creds);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
