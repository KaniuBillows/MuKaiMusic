using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using MuKai_Music.Model.DataEntity;
using MuKai_Music.Model.ResponseEntity;
using MuKai_Music.Model.Service;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System;
using Microsoft.Extensions.Configuration;
using System.Text;
using MusicApi;
using MusicApi.NetEase.User;
using MuKai_Music.Model.Authentication;
using Microsoft.Net.Http.Headers;
using MuKai_Music.Cache;
using Microsoft.AspNetCore.Authentication;

namespace MuKai_Music.Service
{
    public class UserService : ResultOperate
    {
        private readonly HttpContext httpContext;
        private readonly SignInManager<UserInfo> signInManager;
        private readonly AccountManager accountManager;

        protected override IHttpClientFactory HttpClientFactory { get; set; }

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
        public async Task<IResult<string>> Register(UserInfo userInfo)
        {
            if (userInfo.PhoneNumber != null)
            {
                if (await this.accountManager.FindByPhoneNumberAsync(userInfo.PhoneNumber) != null)
                {
                    return new BaseResult<string>(null, 400, "该手机号码已经注册！");
                }
            }
            //将用户保存数据库
            IdentityResult result = await this.accountManager.CreateAsync(userInfo, userInfo.Password);
            if (result.Succeeded)
            {
                /*TODO: 进行邮箱验证*/
                return new BaseResult<string>("注册成功，我们已经向您的邮箱发送验证链接,请前往查看", 200, null);
            }
            else
            {
                IEnumerator<IdentityError> errors = result.Errors.GetEnumerator();
                errors.MoveNext();
                return new BaseResult<string>(null, 500, errors.Current.Description);
            }
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="usr">登录凭据，用户名/邮箱/手机号</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        public async Task<IResult<object>> Login(string usr, string password)
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
            if (userInfo == null) return new BaseResult<object>(null, 401, "用户不存在!");
            SignInResult result = await this.signInManager.CheckPasswordSignInAsync(userInfo, password, false);
            if (result.Succeeded)
            {
                httpContext.Request.Headers.TryGetValue(HeaderNames.UserAgent, out StringValues ua);
                string AccessToken = TokenProvider.CreateAccessToken(userInfo.Id.ToString());
                string RefreshToken = TokenProvider.CreateRefreshToken(userInfo.Id.ToString(), ua.ToString());

                return new BaseResult<object>(new { accessToken = AccessToken, refreshToken = RefreshToken }, 200, null);
            }
            else
            {
                return new BaseResult<object>(null, 401, "登录名或密码错误");
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
        /// 上传头像
        /// </summary>
        /// <param name="avatar">base64格式图片</param>
        /// <returns></returns>
        public async Task<IResult<string>> UploadAvatar(string avatar)
        {
            var user = httpContext.User;

            return await Task.Run(() =>
                  new BaseResult<string>("上传成功", 200, null)
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
            IRequestOption request = new LoginPhone(GetCookie(httpContext.Request), countrycode, phone, password);
            await GetResult(httpContext.Response, request);
        }

        /// <summary>
        /// 注销登录
        /// </summary>
        public async Task Ne_Logout()
        {
            IRequestOption request = new LogOut(GetCookie(httpContext.Request));
            await GetResult(httpContext.Response, request);
        }

        /// <summary>
        /// 获取网易云用户详情信息
        /// </summary>
        /// <param name="id"></param>
        public async Task Ne_GetUserDetail(int id)
        {
            IRequestOption request = new UserDetail(GetCookie(httpContext.Request), id);
            await GetResult(httpContext.Response, request);
        }

        /// <summary>
        /// 获取用户Id
        /// </summary>
        /// <returns></returns>
        private int GetUserId()
        {
            this.httpContext.Request.Headers.TryGetValue("Authentication", out StringValues authentication);
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
            this.httpContext.Request.Headers.TryGetValue("Authentication", out StringValues authentication);
            return authentication.ToString().Substring("Bearer ".Length).Trim();
        }
    }
}
