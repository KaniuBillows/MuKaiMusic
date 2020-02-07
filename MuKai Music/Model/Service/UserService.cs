using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using MuKai_Music.Model.DataEntity;
using MuKai_Music.Model.Manager;
using MuKai_Music.Model.ResponseEntity;
using MuKai_Music.Model.Service;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MuKai_Music.Extensions.Manager;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System;
using Microsoft.Extensions.Configuration;
using System.Text;
using MusicApi;
using MusicApi.NetEase.User;

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
        public async Task<IResult<UserInfo>> Register(UserInfo userInfo)
        {

            if (userInfo.PhoneNumber != null)
            {
                if (await this.accountManager.FindByPhoneNumberAsync(userInfo.PhoneNumber) != null)
                {
                    return new BaseResult<UserInfo>(null, 400, "该手机号码已经注册！");
                }
            }
            //将用户保存数据库
            var result = await this.accountManager.CreateAsync(userInfo, userInfo.Password);
            if (result.Succeeded)
            {
                /*TODO: 进行邮箱验证*/
                var signinResult = await this.signInManager.JwtSignInAsync(userInfo, userInfo.Password, false);
                var user = await this.accountManager.FindByIdAsync(userInfo.Id.ToString());
                //密码
                user.Password = null;
                return HandleResult(user, signinResult);
            }
            else
            {
                IEnumerator<IdentityError> errors = result.Errors.GetEnumerator();
                errors.MoveNext();
                return new BaseResult<UserInfo>(userInfo, 500, errors.Current.Description);
            }
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="usr">登录凭据，用户名/邮箱/手机号</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        public async Task<IResult<UserInfo>> Login(string usr, string password)
        {
            SigninResult result;
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
            if (userInfo == null) return new BaseResult<UserInfo>(null, 401, "用户不存在!");
            result = await this.signInManager.JwtSignInAsync(userInfo, password, false);
            return HandleResult(userInfo, result);
        }

        /// <summary>
        /// 刷新Token
        /// </summary>
        /// <returns></returns>
        public async Task<IResult<string>> RefreshToken()
        {
            httpContext.Request.Headers.TryGetValue("r_token", out StringValues refToken);
            httpContext.Request.Headers.TryGetValue("Authorization", out StringValues Token);
            var handler = new JwtSecurityTokenHandler();
            if (!StringValues.IsNullOrEmpty(refToken))
            {
                try
                {
                    handler.ValidateToken(refToken, new TokenValidationParameters
                    {
                        ValidateIssuer = true,//是否验证Issuer
                        ValidateAudience = true,//是否验证Audience
                        ValidateLifetime = true,//是否验证失效时间
                        ClockSkew = TimeSpan.FromSeconds(30),
                        ValidateIssuerSigningKey = true,//是否验证SecurityKey
                        ValidAudience = Startup.Config.GetValue<string>("Domain"),//Audience
                        ValidIssuer = Startup.Config.GetValue<string>("Domain"),//Issuer，这两项和前面签发jwt的设置一致
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Startup.Config.GetValue<string>("SecurityKey")))
                    }, out SecurityToken securityToken);
                    var token = securityToken as JwtSecurityToken;
                    token.Payload.TryGetValue(ClaimTypes.Name, out object usrname);
                    if (usrname == null) throw new NullReferenceException(nameof(usrname));
                    //颁发新的token
                    return new BaseResult<string>(TokenManager.GetAccessToken(usrname as string), 200, null);
                }
                catch (Exception)
                {
                    return new BaseResult<string>("令牌已过期,请重新登录！", 401, null);
                }
            }
            else
            {
                handler.ValidateToken(Token, new TokenValidationParameters
                {
                    ValidateIssuer = true,//是否验证Issuer
                    ValidateAudience = true,//是否验证Audience
                    ValidateLifetime = true,//是否验证失效时间
                    ClockSkew = TimeSpan.FromSeconds(30),
                    ValidateIssuerSigningKey = true,//是否验证SecurityKey
                    ValidAudience = Startup.Config.GetValue<string>("Domain"),//Audience
                    ValidIssuer = Startup.Config.GetValue<string>("Domain"),//Issuer，这两项和前面签发jwt的设置一致
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Startup.Config.GetValue<string>("SecurityKey")))
                }, out SecurityToken securityToken);
                var token = securityToken as JwtSecurityToken;
                token.Payload.TryGetValue(ClaimTypes.Name, out object usrname);
                await accountManager.SetLockoutEnabledAsync(await accountManager.FindByNameAsync(usrname as string), true);
                return new BaseResult<string>("该账号已被永久封禁！", 403, null);
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
        /// 通过手机号登录
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

        private IResult<UserInfo> HandleResult(UserInfo userInfo, SigninResult signinResult)
        {
            if (signinResult.Succeeded)
            {
                userInfo.Token = signinResult.AccessToken;
                userInfo.Password = null;
                return new BaseResult<UserInfo>(userInfo, 200, null);
            }
            return signinResult.IsNotAllowed
                ? new BaseResult<UserInfo>(null, 403, "此账号不被允许登录")
                : signinResult.IsLockedOut ? new BaseResult<UserInfo>(null, 403, "此账号被锁定") : new BaseResult<UserInfo>(null, 401, "账号或密码错误");
        }
    }
}
