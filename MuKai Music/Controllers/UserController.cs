using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MuKai_Music.Filter;
using MuKai_Music.Service;
using System;
using System.Net.Http;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MuKai_Music.Controllers
{
    [Route("api/[controller]")]
    [ServiceFilter(typeof(MyAuthorFilter))]
    public class UserController : Controller
    {
        public UserService UserService { get; }

        public UserController(MyAuthorFilter myAuthor, Func<HttpContext, UserService> userServiceFactory)
        {
            this.UserService = userServiceFactory.Invoke(myAuthor.HttpContext);
        }

        /// <summary>
        /// 通过手机号码登录网易云
        /// </summary>
        /// <param name="countrycode">手机区号</param>
        /// <param name="phone">手机号码</param>
        /// <param name="password">密码,需客户端采用MD5加密</param>
        /// <returns></returns>
        [HttpPost("netease/login")]
        public async Task LoginPhone(string countrycode, string phone, string password) => await UserService.LogInPhone(countrycode, phone, password);

        /// <summary>
        /// 注销登录网易云
        /// </summary>
        /// <returns></returns>
        [HttpGet("netease/logout")]
        public async Task Logout() => await UserService.Logout();

        /// <summary>
        /// 获取网易云用户详情
        /// </summary>
        /// <param name="id"></param>
        [HttpGet("netase/detail")]
        public async Task GetDetail(int id) => await UserService.GetUserDetail(id);
    }
}
