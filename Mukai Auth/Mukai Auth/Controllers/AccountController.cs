using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DataAbstract;
using DataAbstract.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using Mukai_Auth.DataContext;
using Mukai_Auth.Models;
using Mukai_Auth.Service;

namespace Mukai_Auth.Controllers
{
    [Route("/")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly AccountContext accountContext;
        private readonly TokenProvider tokenProvider;
        private readonly IHttpClientFactory factory;

        public AccountController(AccountContext accountContext,
                                 TokenProvider tokenProvider,
                                 IHttpClientFactory factory)
        {
            this.accountContext = accountContext;
            this.tokenProvider = tokenProvider;
            this.factory = factory;
        }

        /// <summary>
        /// 密码用SHA1加密，且全大写
        /// </summary>
        /// <param name="loginInfo"></param>
        /// <returns></returns>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginModel loginInfo)
        {
            User userInfo;
            var emailRegex = new Regex(@"^[a-zA-Z0-9_-]+@[a-zA-Z0-9_-]+(\.[a-zA-Z0-9_-]+)+$");
            if (emailRegex.IsMatch(loginInfo.LoginName))
            {
                userInfo = await this.accountContext.Users.AsNoTracking().SingleOrDefaultAsync(u => u.Email == loginInfo.LoginName);
            }
            else
            { //验证登录方式是否为手机号码登录
                var phoneRegex = new Regex(@"^1(3[0-9]|4[56789]|5[0-9]|6[6]|7[0-9]|8[0-9]|9[189])\d{8}$");
                userInfo = phoneRegex.IsMatch(loginInfo.LoginName)
                    ? await this.accountContext.Users.AsNoTracking().SingleOrDefaultAsync(u => u.PhoneNumber == loginInfo.LoginName)
                    : await this.accountContext.Users.AsNoTracking().SingleOrDefaultAsync(u => u.UserName == loginInfo.LoginName);
            }
            return ValidPassword(userInfo, loginInfo.PasswordHashed)
                ? new JsonResult(Result<object>.SuccessReuslt(new
                {
                    token = tokenProvider.CreateAccessToken(userInfo.Id.ToString())
                }))
                : new JsonResult(Result<object>.FailResult("用户名或密码错误！", 401));
        }

        [HttpGet("refresh")]
        [Authorize]
        public IActionResult Refresh()
        {
            if (this.HttpContext.Request.Headers.TryGetValue("Authorization", out StringValues authentication))
            {
                string token = authentication.ToString().Substring("Bearer ".Length).Trim();

                var handler = new JwtSecurityTokenHandler();
                try
                {
                    JwtSecurityToken jwtToken = handler.ReadJwtToken(token);
                    string userId = jwtToken.Payload["id"] as string;
                    string reToken = jwtToken.ValidTo - DateTime.UtcNow < TimeSpan.FromDays(3)
                        ? this.tokenProvider.CreateAccessToken(userId) : token;

                    return new JsonResult(Result.SuccessReuslt(new { token = reToken }));
                }
                catch (Exception)
                {

                }
            }
            return this.Challenge();
        }

        [HttpGet("health")]
        public IActionResult Health()
        {
            return Ok();
        }

        private bool ValidPassword(User user, string password)
        {
            return user != null && user.PasswordHash != null && user.PasswordHash.Equals(password);
        }

        private string Sha1(string str)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(str);
            byte[] data = SHA1.Create().ComputeHash(buffer);

            var sb = new StringBuilder();
            foreach (byte t in data)
            {
                sb.Append(t.ToString("X2"));
            }

            return sb.ToString();
        }
    }
}
