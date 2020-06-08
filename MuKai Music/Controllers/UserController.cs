using DataAbstract;
using DataAbstract.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using MuKai_Account;
using MuKai_Music.Attribute;
using MuKai_Music.Attributes;
using MuKai_Music.Cache;
using MuKai_Music.Filter;
using MuKai_Music.Model.RequestParam;
using MuKai_Music.Service;
using System;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Text;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MuKai_Music.Controllers
{
    [Route("api/account")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly AccountService.AccountServiceClient client;
        private readonly TokenProvider tokenProvider;
        private readonly RedisClient redisClient;
        private readonly IConfiguration config;

        public UserController(AccountService.AccountServiceClient client,
            TokenProvider tokenProvider, RedisClient redisClient, IConfiguration config)
        {
            this.client = client;
            this.tokenProvider = tokenProvider;
            this.redisClient = redisClient;
            this.config = config;
        }

        /// <summary>
        /// 注册用户
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        /// //[Encrypt]
        [HttpPost("register")]
        [ResponseCache(NoStore = true)]
        [ApiCache(NoStore = true)]
        [Encrypt]
        [AllowAnonymous]
        public async Task<Result> Register([FromBody] User userInfo)
        {
            try
            {
                if (userInfo.AvatarUrl == null)
                {
                    userInfo.AvatarUrl = "https://pic.kaniu.pro/mukai/avatar-default.jpg";
                }

                RegisterReply reply = await this.client.RegisterAsync(new RegisterRequest()
                {
                    UserName = userInfo.UserName,
                    AvatarUrl = userInfo.AvatarUrl,
                    Email = userInfo.Email,
                    PassWord = userInfo.Password,
                    NickName = userInfo.NickName,
                    PhoneNumber = userInfo.PhoneNumber
                });
                return reply.Success ? Result.SuccessReuslt(reply.Message) :
                    Result.FailResult(reply.Message, 400);
            }
            catch (Exception)
            {
                return Result.FailResult("服务当前不可用", 503);
            }
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <returns></returns>
        [HttpPost("login")]
        [ResponseCache(NoStore = true)]
        [ApiCache(NoStore = true)]
        [Encrypt]
        [AllowAnonymous]
        public async Task<Result<object>> LogIn([FromBody] LoginParam param)
        {
            LoginReply reply;
            try
            {
                reply = await this.client.LoginAsync(new LoginRequest()
                {
                    UserName = param.UserName,
                    PassWord = param.Password
                });
            }
            catch (Exception)
            {
                return Result.FailResult("服务当前不可用", 503);

            }
            if (reply.Success)
            {
                this.HttpContext.Request.Headers.TryGetValue(HeaderNames.UserAgent, out StringValues ua);
                return Result<object>.SuccessReuslt(new
                {
                    accessToken = this.tokenProvider.CreateAccessToken(reply.Id.ToString()),
                    refreshToken = this.tokenProvider.CreateRefreshToken(reply.Id.ToString(), ua)
                });
            }
            else
            {
                return Result.FailResult(reply.Message);
            }
        }

        /// <summary>
        /// 登出
        /// </summary>
        /// <returns></returns>
        [HttpGet("logout")]
        [ResponseCache(NoStore = true)]
        [ApiCache(NoStore = true)]
        [Authorization]
        public async Task<IActionResult> LogOut(string token)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                JwtSecurityToken jwtToken = handler.ReadJwtToken(token);
                TimeSpan expiry = jwtToken.ValidTo - new DateTime(1970, 1, 1, 0, 0, 0);
                await this.redisClient.SetStringKeyAsync(token, token, expiry);
                return Ok();
            }
            catch (Exception)
            {
            }
            return Forbid();
        }

        /// <summary>
        /// 获取用户信息,通过Id
        /// </summary>
        /// <param name="loginUserId">由Filter自动注入</param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("info")]
        [ResponseCache(Duration = 86400)]
        //[Authorization]
        [AllowAnonymous]
        public async Task<Result<User>> UserInfo(long loginUserId, long? id)
        {
            try
            {
                var reply = await this.client.GetUserInfoAsync(new UserInfoRequest()
                {
                    Id = id ?? loginUserId
                });
                return reply.UserInfo != null
                    ? Result<User>.SuccessReuslt(new User()
                    {
                        Id = reply.UserInfo.Id,
                        UserName = reply.UserInfo.UserName,
                        NickName = reply.UserInfo.NickName,
                        Email = reply.UserInfo.Email,
                        PhoneNumber = reply.UserInfo.PhoneNumber,
                        AvatarUrl = reply.UserInfo.AvatarUrl
                    })
                    : Result<User>.FailResult(reply.Message, 400);
            }
            catch (Exception e)
            {
                return Result<User>.FailResult("当前服务不可用", 503);
            }
        }

        /// <summary>
        /// 上传头像
        /// </summary>
        /// <param name="loginUserId"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost("upload/avatar")]
        [ResponseCache(NoStore = true)]
        [Authorization]
        public async Task<Result<string>> UploadAvatar(long loginUserId, [Required] IFormFile file)
        {
            long size = config.GetMaxPicSize();
            if (size < file.Length / 1024)
            {
                return Result<string>.FailResult("文件大小超过限制,最大支持2m,jpg,png,gif格式", 400);
            }
            else
            {
                if (file.ContentType == "image/jpeg" || file.ContentType == "image/png" || file.ContentType == "image/gif")
                {
                    StringBuilder path = new StringBuilder(config.GetPicRootPath());
                    string fileName = $"avatar-{loginUserId}.{file.ContentType.Substring(6)}";
                    string filePath = path + fileName;
                    if (System.IO.File.Exists(filePath))
                    {
                        //先备份原头像
                        FileInfo fi = new FileInfo(filePath);
                        fi.MoveTo(path + "old" + fileName);
                    }
                    using var stream = System.IO.File.Create(filePath);
                    await file.CopyToAsync(stream);
                    var url = $"https://pic.kaniu.pro/mukai/{fileName}";
                    //更新数据库
                    try
                    {
                        var reply = await this.client.UpdateAvatorAsync(new UpdateAvatorRequest()
                        {
                            Id = loginUserId,
                            NewUrl = url
                        });
                        if (reply.Success)
                        {
                            //删除原头像
                            System.IO.File.Delete(path + "old" + fileName);
                            return Result<string>.SuccessReuslt(url);
                        }
                        else
                        {
                            //还原头像
                            System.IO.File.Delete(filePath);
                            FileInfo fi = new FileInfo(path + "old" + fileName);
                            fi.MoveTo(filePath);
                            return Result<string>.FailResult("服务器出错了，请稍后再试", 500);
                        }
                    }
                    catch (Exception)
                    {
                        //还原头像
                        System.IO.File.Delete(filePath);
                        FileInfo fi = new FileInfo(path + "old" + fileName);
                        fi.MoveTo(filePath);
                        return Result<string>.FailResult("服务当前不可用", 503);
                    }
                }
                else
                {
                    return Result<string>.FailResult("不支持的文件格式，请选择jpg,png或gif格式", 400);
                }
            }
        }


        /// <summary>
        /// 通过手机号码登录网易云
        /// </summary>
        /// <param name="countrycode">手机区号</param>
        /// <param name="phone">手机号码</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        [HttpGet("netease/login")]
        [ResponseCache(NoStore = true)]
        [ApiCache(NoStore = true)]
        public async Task LoginPhone(string countrycode, string phone, string password)
        {

        }


        /// <summary>
        /// 获取网易云用户详情
        /// </summary>
        /// <param name="id"></param>
        [HttpGet("netase/detail")]
        [ApiCache(Duration = 3600)]
        public async Task GetDetail(int id)
        {

        }


    }
}
