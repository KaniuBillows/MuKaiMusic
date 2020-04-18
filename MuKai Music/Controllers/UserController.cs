using DataAbstract;
using DataAbstract.Account;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using MuKai_Account;
using MuKai_Music.Attribute;
using MuKai_Music.Attributes;
using MuKai_Music.Cache;
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
    [Route("api")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly AccountService.AccountServiceClient client;
        private readonly TokenProvider tokenProvider;
        private readonly RedisClient redisClient;

        public UserController(AccountService.AccountServiceClient client,
            TokenProvider tokenProvider, RedisClient redisClient)
        {
            this.client = client;
            this.tokenProvider = tokenProvider;
            this.redisClient = redisClient;
        }

        /// <summary>
        /// 注册用户
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        /// //[Encrypt]
        [HttpPost("account/register")]
        [ResponseCache(NoStore = true)]
        [ApiCache(NoStore = true)]
        [AllowAnonymous]
        public async Task<Result<string>> Register([FromBody] User userInfo)
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
                return reply.Success ? new Result<string>(reply.Message, 200, null) :
                    new Result<string>(null, 400, reply.Message);
            }
            catch (Exception)
            {
                return new Result<string>(null, 503, "服务当前不可用");
            }
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <returns></returns>
        [HttpPost("account/login")]
        [ResponseCache(NoStore = true)]
        [ApiCache(NoStore = true)]
        [Encrypt]
        [AllowAnonymous]
        public async Task<Result<object>> LogIn([FromBody]LoginParam param)
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
                return new Result<object>(null, 503, "服务当前不可用");

            }
            if (reply.Success)
            {
                this.HttpContext.Request.Headers.TryGetValue(HeaderNames.UserAgent, out StringValues ua);
                return new Result<object>(new
                {
                    accessToken = this.tokenProvider.CreateAccessToken(reply.Id.ToString()),
                    refreshToken = this.tokenProvider.CreateRefreshToken(reply.Id.ToString(), ua)
                }, 200, null);
            }
            else
            {
                return new Result<object>(null, 500, reply.Message);
            }
        }

        /// <summary>
        /// 登出
        /// </summary>
        /// <returns></returns>
        [HttpGet("account/logout")]
        [ResponseCache(NoStore = true)]
        [ApiCache(NoStore = true)]
        public async Task<IActionResult> LogOut([Required]int id)
        {
            if (this.HttpContext.Request.Headers.TryGetValue("Authorization", out StringValues authentication))
            {
                string token = authentication.ToString().Substring("Bearer ".Length).Trim();
                var handler = new JwtSecurityTokenHandler();
                JwtSecurityToken jwtToken = handler.ReadJwtToken(token);
                if (!jwtToken.Payload.TryGetValue("id", out object tid) || !id.ToString().Equals(tid as string))
                {
                    return Forbid();
                }
                TimeSpan expiry = jwtToken.ValidTo - new DateTime(1970, 1, 1, 0, 0, 0);
                await this.redisClient.SetStringKeyAsync(token, token, expiry);
                return Ok();
            }
            return Forbid();
        }

        /// <summary>
        /// 获取用户信息,通过Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("account/info")]
        [ResponseCache(Duration = 86400)]
        public async Task<Result<User>> UserInfo(int? id)
        {
            if (!id.HasValue)
            {
                this.HttpContext.Request.Headers.TryGetValue("Authorization", out StringValues authentication);
                string token = authentication.ToString().Substring("Bearer ".Length).Trim();
                var handler = new JwtSecurityTokenHandler();
                JwtSecurityToken jwtToken = handler.ReadJwtToken(token);
                if (!jwtToken.Payload.TryGetValue("id", out object tid) || !id.ToString().Equals(tid as string))
                {
                    await this.HttpContext.ForbidAsync();
                    return null;
                }
                id = (int)tid;
            }
            try
            {

                var reply = await this.client.GetUserInfoAsync(new UserInfoRequest()
                {
                    Id = id.Value
                });
                return reply.UserInfo != null
                    ? new Result<User>(new User()
                    {
                        Id = reply.UserInfo.Id,
                        UserName = reply.UserInfo.UserName,
                        NickName = reply.UserInfo.NickName,
                        Email = reply.UserInfo.Email,
                        PhoneNumber = reply.UserInfo.PhoneNumber,
                        AvatarUrl = reply.UserInfo.AvatarUrl
                    }, 200, null)
                    : new Result<User>(null, 400, reply.Message);
            }
            catch (Exception)
            {
                return new Result<User>(null, 503, "当前服务不可用");
            }
        }

        /// <summary>
        /// 上传头像
        /// </summary>
        /// <param name="id"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost("account/upload/avatar")]
        [ResponseCache(NoStore = true)]
        [AllowAnonymous]
        public async Task<Result<string>> UploadAvatar([Required]int id, [Required] IFormFile file)
        {
            #region 验证用户Id
            this.HttpContext.Request.Headers.TryGetValue("Authorization", out StringValues authentication);
            string token = authentication.ToString().Substring("Bearer ".Length).Trim();
            var handler = new JwtSecurityTokenHandler();
            JwtSecurityToken jwtToken = handler.ReadJwtToken(token);
            if (!jwtToken.Payload.TryGetValue("id", out object tid) || !id.ToString().Equals(tid as string))
            {
                await this.HttpContext.ForbidAsync();
                return null;
            }
            #endregion
            long size = Startup.Configuration.GetMaxPicSize();
            if (size < file.Length / 1024)
            {
                return new Result<string>(null, 400, "文件大小超过限制,最大支持2m,jpg,png,gif格式");
            }
            else
            {
                if (file.ContentType == "image/jpeg" || file.ContentType == "image/png" || file.ContentType == "image/gif")
                {
                    StringBuilder path = new StringBuilder(Startup.Configuration.GetPicRootPath());
                    string fileName = $"avatar-{id}.{file.ContentType.Substring(6)}";
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
                            Id = id,
                            NewUrl = url
                        });
                        if (reply.Success)
                        {
                            //删除原头像
                            System.IO.File.Delete(path + "old" + fileName);
                            return new Result<string>(url, 200, null);
                        }
                        else
                        {
                            //还原头像
                            System.IO.File.Delete(filePath);
                            FileInfo fi = new FileInfo(path + "old" + fileName);
                            fi.MoveTo(filePath);
                            return new Result<string>(null, 500, "服务器出错了，请稍后再试");
                        }
                    }
                    catch (Exception)
                    {
                        //还原头像
                        System.IO.File.Delete(filePath);
                        FileInfo fi = new FileInfo(path + "old" + fileName);
                        fi.MoveTo(filePath);
                        return new Result<string>(null, 503, "服务当前不可用");
                    }
                }
                else
                {
                    return new Result<string>(null, 400, "不支持的文件格式，请选择jpg,png或gif格式");
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
