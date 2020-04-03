using DataAbstract;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using MuKai_Music.Cache;
using MuKai_Music.Model.Authentication;
using MuKai_Music.Model.DataEntity;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MuKai_Music.Service
{
    public class UserService
    {
        private readonly HttpContext httpContext;
        private readonly SignInManager<UserInfo> signInManager;
        private readonly AccountManager accountManager;

        protected IHttpClientFactory HttpClientFactory { get; set; }

        public UserService(HttpContext httpContext,
            SignInManager<UserInfo> signInManager,
            IHttpClientFactory httpClientFactory,
            AccountManager accountManager)
        {
            this.httpContext = httpContext;
            this.signInManager = signInManager;
            this.HttpClientFactory = httpClientFactory;
            this.accountManager = accountManager;
        }

        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        public async Task<Result<string>> Register(UserInfo userInfo)
        {
            if (userInfo.PhoneNumber != null)
            {
                if (await this.accountManager.FindByPhoneNumberAsync(userInfo.PhoneNumber) != null)
                {
                    return new Result<string>(null, 400, "该手机号码已经注册！");
                }
            }
            //将用户保存数据库
            IdentityResult result = await this.accountManager.CreateAsync(userInfo, userInfo.Password);
            if (result.Succeeded)
            {
                /*TODO: 进行邮箱验证*/
                return new Result<string>("注册成功，我们已经向您的邮箱发送验证链接,请前往查看", 200, null);
            }
            else
            {
                IEnumerator<IdentityError> errors = result.Errors.GetEnumerator();
                errors.MoveNext();
                return new Result<string>(null, 500, errors.Current.Description);
            }
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="usr">登录凭据，用户名/邮箱/手机号</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        public async Task<Result<object>> Login(string usr, string password)
        {
            UserInfo userInfo;
            //验证登录方式是否为邮箱
            var emailRegex = new Regex(@"^[a-zA-Z0-9_-]+@[a-zA-Z0-9_-]+(\.[a-zA-Z0-9_-]+)+$");
            if (emailRegex.IsMatch(usr))
            {
                userInfo = await this.accountManager.FindByEmailAsync(usr);
            }
            else
            { //验证登录方式是否为手机号码登录
                var phoneRegex = new Regex(@"^1(3[0-9]|4[56789]|5[0-9]|6[6]|7[0-9]|8[0-9]|9[189])\d{8}$");
                userInfo = phoneRegex.IsMatch(usr)
                    ? await this.accountManager.FindByPhoneNumberAsync(usr)
                    : await this.accountManager.FindByNameAsync(usr);
            }
            if (userInfo == null) return new Result<object>(null, 401, "用户不存在!");
            if (RedisClient.RedisClientInstence.Exists(userInfo.Id.ToString()))
            {
                await httpContext.ForbidAsync();
                return null;
            }
            SignInResult result = await this.signInManager.CheckPasswordSignInAsync(userInfo, password, false);
            if (result.Succeeded)
            {
                httpContext.Request.Headers.TryGetValue(HeaderNames.UserAgent, out StringValues ua);
                string AccessToken = TokenProvider.CreateAccessToken(userInfo.Id.ToString());
                string RefreshToken = TokenProvider.CreateRefreshToken(userInfo.Id.ToString(), ua.ToString());
                return new Result<object>(new { accessToken = AccessToken, refreshToken = RefreshToken }, 200, null);
            }
            else
            {
                return new Result<object>(null, 401, "登录名或密码错误");
            }
        }

        /// <summary>
        /// 更改用户密码，将用户Id存入Redis中，TokenManger会阻止该Id的所有调用
        /// 必须重新登录来解除限制
        /// </summary>
        /// <param name="usrId"></param>
        /// <param name="oldPassword"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        public async Task<Result<string>> ChangePassword(int usrId, string oldPassword, string newPassword)
        {
            if (oldPassword.Equals(newPassword)) return new Result<string>(null, 400, "输入的新密码和旧密码是同一个");
            var regex = new Regex(@"^[\w_-]{6,16}$");
            if (!regex.IsMatch(newPassword)) return new Result<string>(null, 400, "密码为英文或数字组合，可以包含\"-\"与\"_\"，长度在8-16个字符之间");
            UserInfo user = await this.accountManager.FindByIdAsync(usrId.ToString());
            if (user == null) return new Result<string>(null, 400, "用户不存在!");
            if (user.Id != GetUserId()) return new Result<string>(null, 400, "What are you fucking doing?");
            bool oldPwdRight = await this.accountManager.CheckPasswordAsync(user, oldPassword);
            if (!oldPwdRight)
            {
                return new Result<string>(null, 400, "原密码输入不正确！");
            }
            else
            {
                IdentityResult indentyResult = await this.accountManager.ChangePasswordAsync(user, oldPassword, newPassword);
                if (indentyResult.Succeeded)
                {
                    await RedisClient.RedisClientInstence.SetStringKeyAsync(user.Id.ToString(), user.Id.ToString());
                    return new Result<string>("更改成功，请重新登录！", 200, null);
                }
                else
                {
                    return new Result<string>(null, 500, "暂时无法更改您的密码");
                }
            }
        }

        /// <summary>
        /// 账户注销
        /// </summary>
        /// <returns></returns>
        public async Task Logout()
        {
            try
            {
                this.httpContext.Request.Headers.TryGetValue("Authentication", out StringValues authentication);
                string token = authentication.ToString().Substring("Bearer ".Length).Trim();
                var handler = new JwtSecurityTokenHandler();
                handler.ValidateToken(token, Startup.TokenValidationParameters, out SecurityToken securityToken);
                var jwtToken = securityToken as JwtSecurityToken;
                this.httpContext.Request.Headers.TryGetValue(HeaderNames.UserAgent, out StringValues ua);
                int userId = int.Parse(jwtToken.Payload["id"].ToString());
                string RefreshKey = TokenProvider.GetTokenKey(userId.ToString(), ua.ToString());
                //根据Id和ua，删除Redis中的RefreshToken
                await RedisClient.RedisClientInstence.DeleteKeyAsync(RefreshKey);
                await RedisClient.RedisClientInstence.SetStringKeyAsync(GetAccessToken(), userId, TimeSpan.FromSeconds(jwtToken.Payload.Exp.Value));
            }
            catch
            {
                return;
            }
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <returns></returns>
        public async Task<Result<UserInfo>> GetUserInfo(int? id)
        {
            if (id.HasValue) return new Result<UserInfo>(await this.accountManager.FindByIdAsync(id.ToString()), 200, null);
            int? userId = GetUserId();
            return !userId.HasValue
                ? new Result<UserInfo>(null, 404, "未找到用户")
                : new Result<UserInfo>(await this.accountManager.FindByIdAsync(userId.ToString()), 200, null);
        }

        /// <summary>
        /// 上传头像
        /// </summary>
        /// <param name="avatar">base64格式图片</param>
        /// <returns></returns>
        public async Task<Result<string>> UploadAvatar(string avatar)
        {
            return await Task.Run(() =>
                  new Result<string>("上传成功", 200, null)
              );
        }


        /// <summary>
        /// 通过手机号登录网易
        /// </summary>
        /// <param name="countrycode"></param>
        /// <param name="phone"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task Ne_LogInPhone(string countrycode, string phone, string password)
        {
           
        }

        /// <summary>
        /// 注销登录
        /// </summary>
        public async Task Ne_Logout()
        {
          
        }

        /// <summary>
        /// 获取网易云用户详情信息
        /// </summary>
        /// <param name="id"></param>
        public async Task Ne_GetUserDetail(int id)
        {
         
        }

        /// <summary>
        /// 获取用户Id
        /// </summary>
        /// <returns></returns>
        private int? GetUserId()
        {
            if (!this.httpContext.Request.Headers.TryGetValue("Authorization", out StringValues authentication))
                return null;
            string token = authentication.ToString().Substring("Bearer ".Length).Trim();
            var handler = new JwtSecurityTokenHandler();
            JwtSecurityToken jwtToken = handler.ReadJwtToken(token);
            return int.Parse(jwtToken.Payload["id"] as string);
        }

        /// <summary>
        /// 获取accessToken
        /// </summary>
        /// <returns></returns>
        private string GetAccessToken()
        {
            this.httpContext.Request.Headers.TryGetValue("Authorization", out StringValues authentication);
            return authentication.ToString().Substring("Bearer ".Length).Trim();
        }

        /// <summary>
        /// 获取用户UA
        /// </summary>
        /// <returns></returns>
        private string GetUserAgent()
        {
            if (this.httpContext.Request.Headers.TryGetValue(HeaderNames.UserAgent, out StringValues ua))
            {
                return ua.ToString();
            }
            else
            {
                return null;
            }
        }
    }
}
