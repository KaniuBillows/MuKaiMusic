using DataAbstract;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using MuKai_Music.Cache;
using MuKai_Music.Service;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MuKai_Music.Middleware.TokenManager
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class TokenManagerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly RedisClient _redisClient;
        private readonly TokenProvider tokenProvider;

        public TokenManagerMiddleware(RequestDelegate next, RedisClient redis, TokenProvider tokenProvider)
        {
            this._redisClient = redis;
            this.tokenProvider = tokenProvider;
            _next = next;
        }

        /// <summary>
        /// 只要请求头添加Re_T value为RefreshToken，则视为刷新AccessToken，在验证之后进行管道短路
        /// 如果包含Authentication，则验证此token是否位于黑名单(由登出产生),或者是否使用RefreshToken当作AccessToken
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public async Task InvokeAsync(HttpContext httpContext)
        {
            //读取Ua信息
            if (!httpContext.Request.Headers.TryGetValue(HeaderNames.UserAgent, out StringValues ua))
            {
                await httpContext.ForbidAsync();
                return;
            }
            if (httpContext.Request.Headers.TryGetValue("Re_T", out StringValues refreshToken))
            {
                var handler = new JwtSecurityTokenHandler();
                try
                {
                    //验证此RefreshToken是否有效
                    handler.ValidateToken(refreshToken, Startup.TokenValidationParameters, out SecurityToken token);
                    //从token中读取用户Id
                    string userId = (token as JwtSecurityToken).Payload["id"] as string;
                    if (this._redisClient.Exists(userId))
                    {
                        await httpContext.ChallengeAsync();
                        return;
                    }
                    //检查RefreshToken是否在Redis中，如果不在，则视为用户已登出，返回401
                    string key = TokenProvider.GetTokenKey(userId, ua);
                    if (!this._redisClient.Exists(key))
                    {
                        await httpContext.ChallengeAsync();
                        return;
                    }
                    //如果RefreshToken将在3天内过期，生成新的RefreshToken
                    string reToken = (token as JwtSecurityToken).ValidTo - DateTime.UtcNow < TimeSpan.FromDays(3)
                        ? this.tokenProvider.CreateRefreshToken(userId as string, ua)
                        : (string)refreshToken;
                    //生成新的返回内容
                    string content = JsonSerializer.Serialize<Result<object>>(Result<object>.SuccessReuslt(
                        new
                        {
                            accessToken = this.tokenProvider.CreateAccessToken(userId as string),
                            refreshToken = reToken
                        }), Startup.JsonSerializerOptions);
                    byte[] buffer = Encoding.UTF8.GetBytes(content);
                    httpContext.Response.ContentType = "application/json; charset=utf-8";
                    Task writeBodyTask = httpContext.Response.Body.WriteAsync(buffer, 0, buffer.Length);
                    return;
                }
                catch
                {
                    await httpContext.ForbidAsync();
                    return;
                }
            }
            if (httpContext.Request.Headers.TryGetValue("Authorization", out StringValues authentication))
            {
                string token = authentication.ToString().Substring("Bearer ".Length).Trim();
                //黑名单
                if (this._redisClient.Exists(token))
                {
                    await httpContext.ForbidAsync();
                    return;
                }
                var handler = new JwtSecurityTokenHandler();
                try
                {
                    JwtSecurityToken jwtToken = handler.ReadJwtToken(token);
                    //如果Redis中存在userId,用户更改密码之后，要求重新登录
                    if (this._redisClient.Exists(jwtToken.Payload["id"] as string))
                    {
                        await httpContext.ChallengeAsync();
                        return;
                    }
                    if (jwtToken.Payload["type"] as string != "ac")
                    {
                        await httpContext.ForbidAsync();
                        return;
                    }
                }
                catch (Exception)
                {
                    await httpContext.ForbidAsync();
                    return;
                }


            }
            await _next(httpContext);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class TokenManagerMiddlewareExtensions
    {
        public static IApplicationBuilder UseTokenManager(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<TokenManagerMiddleware>();
        }
    }
}
