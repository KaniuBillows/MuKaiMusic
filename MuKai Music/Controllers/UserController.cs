using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MuKai_Music.Attribute;
using MuKai_Music.Model.DataEntity;
using MuKai_Music.Model.Manager;
using MuKai_Music.Model.ResponseEntity;
using MuKai_Music.Service;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MuKai_Music.Controllers
{
    [Route("api")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly UserService userService;

        public IHttpContextAccessor HttpContextAccessor { get; }

        public UserController(
            SignInManager<UserInfo> signInManager,
            IHttpContextAccessor httpContextAccessor,
            AccountManager accountManager)
        {
            this.HttpContextAccessor = httpContextAccessor;
            this.userService = new UserService(HttpContextAccessor.HttpContext, signInManager, accountManager);
        }

        /// <summary>
        /// 注册用户
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        [HttpPost("account/register")]
        [ResponseCache(NoStore = true)]
        [ApiCache(NoStore = true)]
        [AllowAnonymous]
        public async Task<IResult<UserInfo>> Register([FromBody] UserInfo userInfo)
            => await this.userService.Register(userInfo);

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [HttpPost("account/login")]
        [ResponseCache(NoStore = true)]
        [ApiCache(NoStore = true)]
        [AllowAnonymous]
        public async Task<IResult<UserInfo>> Login([Required][FromForm]string username, [Required][FromForm] string password)
            => await this.userService.Login(username, password);

        /// <summary>
        /// 上传头像
        /// </summary>
        /// <param name="fileData"></param>
        /// <returns></returns>
        [HttpPost("account/upload/avatar")]
        [ResponseCache(NoStore = true)]
        [ApiCache(NoStore = true)]
        public async Task<IResult<string>> UploadAvatar(string fileData) => await this.userService.UploadAvatar(fileData);

        [HttpPut("account/verification")]
        [ApiCache(NoStore = true)]
        [ResponseCache(NoStore = true)]
        public async Task<IResult<string>> RefreshToken() => await userService.RefreshToken();

        /// <summary>
        /// 通过手机号码登录网易云
        /// </summary>
        /// <param name="countrycode">手机区号</param>
        /// <param name="phone">手机号码</param>
        /// <param name="password">密码,需客户端采用MD5加密</param>
        /// <returns></returns>
        [HttpGet("netease/login")]
        [ResponseCache(NoStore = true)]
        [ApiCache(NoStore = true)]
        public async Task LoginPhone(string countrycode, string phone, string password) => await userService.Ne_LogInPhone(countrycode, phone, password);

        /// <summary>
        /// 注销登录网易云
        /// </summary>
        /// <returns></returns>
        [HttpGet("netease/logout")]
        [ApiCache(NoStore = true)]
        [ResponseCache(NoStore = true)]
        public async Task Logout() => await userService.Ne_Logout();

        /// <summary>
        /// 获取网易云用户详情
        /// </summary>
        /// <param name="id"></param>
        [HttpGet("netase/detail")]
        [ApiCache(Duration = 3600)]
        public async Task GetDetail(int id) => await userService.Ne_GetUserDetail(id);


    }
}
