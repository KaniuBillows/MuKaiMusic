using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using MuKai_Music.Cache;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using MuKai_Music.Service;
using MuKai_Music.Model.ResponseEntity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.Extensions.DependencyInjection;

namespace MuKai_Music.Middleware.TokenManager
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class TokenManagerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IAuthorizationPolicyProvider _policyProvider;
        private readonly RedisClient _redisClient;
        public TokenManagerMiddleware(RequestDelegate next, IAuthorizationPolicyProvider policyProvider)
        {
            this._redisClient = RedisClient.RedisClientInstence;
            _next = next;
            this._policyProvider = policyProvider;
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
                    //检查RefreshToken是否在Redis中，如果不在，则视为用户已登出，返回401
                    if (!this._redisClient.Exists(TokenProvider.GetTokenKey(userId, ua)))
                    {
                        await httpContext.ChallengeAsync();
                        return;
                    }
                    //如果RefreshToken将在3天内过期，生成新的RefreshToken
                    string reToken = (token as JwtSecurityToken).ValidTo - DateTime.UtcNow < TimeSpan.FromDays(3)
                        ? TokenProvider.CreateRefreshToken(userId as string, ua)
                        : (string)refreshToken;
                    //生成新的返回内容
                    string content = JsonSerializer.Serialize(new BaseResult<object>(
                        new
                        {
                            accessToken = TokenProvider.CreateAccessToken(userId as string),
                            refreshToken = reToken
                        }, 200, null));
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
            if (httpContext.Request.Headers.TryGetValue("Authentication", out StringValues authentication))
            {
                string token = authentication.ToString().Substring("Bearer ".Length).Trim();
                if (this._redisClient.Exists(token))
                {
                    await httpContext.ForbidAsync();
                    return;
                }
                var handler = new JwtSecurityTokenHandler();
                JwtSecurityToken jwtToken = handler.ReadJwtToken(token);
                if (jwtToken.Payload["type"] as string != "ac")
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
