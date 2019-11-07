using Microsoft.AspNetCore.Http;
using MuKai_Music.Model.Service;
using NetEaseMusic_API.RequestOption.Options.User;
using RequestHandler;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MuKai_Music.Service
{
    public class UserService : ResultOperate
    {
        private readonly HttpContext httpContext;

        public UserService(HttpContext httpContext)
        {
            this.httpContext = httpContext;
        }

        /// <summary>
        /// 通过手机号登录
        /// </summary>
        /// <param name="countrycode"></param>
        /// <param name="phone"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task LogInPhone(string countrycode, string phone, string password)
        {
            IRequestOption request = new LoginPhone(GetCookie(httpContext.Request), countrycode, phone, password);
            await GetResult(httpContext.Response, request);
        }

        /// <summary>
        /// 注销登录
        /// </summary>
        public async Task Logout()
        {
            IRequestOption request = new LogOut(GetCookie(httpContext.Request));
            await GetResult(httpContext.Response, request);
        }

        /// <summary>
        /// 获取用户详情信息
        /// </summary>
        /// <param name="id"></param>
        public async Task GetUserDetail(int id)
        {
            IRequestOption request = new UserDetail(GetCookie(httpContext.Request), id);
            await GetResult(httpContext.Response, request);
        }
    }
}
