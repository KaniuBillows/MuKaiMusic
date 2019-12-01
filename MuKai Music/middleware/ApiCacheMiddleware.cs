using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;

namespace MuKai_Music.Middleware
{
    public enum CacheType
    {
        Redis,
        Memory
    }
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class ApiCacheMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly MemoryCacheEntryOptions _options;
        public CacheType CacheType { get; private set; }
        public ApiCacheMiddleware(RequestDelegate next,
            CacheType cacheType,
            MemoryCacheEntryOptions options)
        {
            if (cacheType == CacheType.Redis)
            {
                throw new NotImplementedException("Redis Cache is Not Implemented!");
            }
            _next = next;
            _options = options;
            CacheType = cacheType;
        }

        public async Task InvokeAsync(HttpContext httpContext,
            IMemoryCache memoryCache,
            IWebHostEnvironment env)
        {

            //暂存原始响应流
            var originResponseStream = httpContext.Response.Body;
            //在内存中开辟缓冲区暂存请求体
            httpContext.Request.EnableBuffering();
            using var requestReader = new StreamReader(httpContext.Request.Body);
            string key = await RequestHandle(requestReader, httpContext);
            if (key == null)
            {
                await _next(httpContext);
                return;
            }
            httpContext.Request.Body.Position = 0;
            if (Check(key, out string value, memoryCache))
            {
                var item = httpContext.Response.HasStarted;
                byte[] buffer = Encoding.UTF8.GetBytes(value);
                httpContext.Response.ContentType = "application/json; charset=utf-8";
                httpContext.Response.Headers.Add("Api-Cache", "hit");
                httpContext.Response.Headers.Add("Api-Cache-Time", _options.AbsoluteExpirationRelativeToNow.Value.TotalSeconds.ToString());
                /*if (env.IsDevelopment())
                {
                    httpContext.Response.Headers.Add("Api-Cache", CacheType.ToString());
                }*/
                await httpContext.Response.Body.WriteAsync(buffer, 0, buffer.Length);
                return;
            }
            else
            {
                httpContext.Response.Headers.Add("Api-Cache", "miss");
                httpContext.Response.Headers.Add("Api-Cache-Add", "true");
                //将原本不可读取的响应流替换为可读
                using var resBody = new MemoryStream();
                httpContext.Response.Body = resBody;
                await _next(httpContext);
                using var responseReader = new StreamReader(resBody);
                resBody.Position = 0;
                await Cache(key, await ResponseHandle(responseReader, httpContext), memoryCache);
                resBody.Position = 0;
                await resBody.CopyToAsync(originResponseStream);
                httpContext.Response.Body = originResponseStream;
            }


        }

        /// <summary>
        /// 根据请求信息生成Key
        /// </summary>
        /// <param name="requestReader"></param>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        private async Task<string> RequestHandle(StreamReader requestReader, HttpContext httpContext)
        {
            var stringBuilder = new StringBuilder(httpContext.Request.GetDisplayUrl());
            if (!stringBuilder.ToString().Contains("/api"))
            {
                return null;
            }
            //TODO post传入多个参数，对每个参数都生成key
            string requestContent = await requestReader.ReadToEndAsync();
            string key = stringBuilder.Append(requestContent).ToString();
            return key;
        }

        /// <summary>
        /// 处理响应结果
        /// </summary>
        /// <param name="responseReader"></param>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        private async Task<string> ResponseHandle(StreamReader responseReader, HttpContext httpContext)
        {
            string responseContent = await responseReader.ReadToEndAsync();
            if (httpContext.Response.ContentType != "application/json; charset=utf-8")
            {
                return null;
            }
            else
            {
                return responseContent;
            }
        }

        /// <summary>
        /// 检查缓存中是否存在key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="memoryCache"></param>
        /// <returns></returns>
        private bool Check(string key, out string value, IMemoryCache memoryCache)
        {
            return memoryCache.TryGetValue(key, out value);
        }

        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="memoryCache"></param>
        /// <returns></returns>
        private async Task Cache(string key, string value, IMemoryCache memoryCache)
        {
            await Task.Run(() =>
              memoryCache.Set(key, value, _options));
        }


    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class ApiCacheMiddlewareExtensions
    {
        /// <summary>
        /// 手动进行缓存设置
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="cacheType"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseApiCacheMiddleware(this IApplicationBuilder builder,
            CacheType cacheType,
            Action<MemoryCacheEntryOptions> options)
        {
            var cacheOptions = new MemoryCacheEntryOptions();
            options(cacheOptions);
            return builder.UseMiddleware<ApiCacheMiddleware>(cacheOptions, cacheType);
        }

        /// <summary>
        /// 读取配置文件设置缓存
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseApiCacheMiddleware(this IApplicationBuilder builder,
            IConfiguration configuration)
        {
            string age = configuration.GetSection("cache-age")?.Value;
            string cacheType = configuration.GetSection("cache-type")?.Value;
            if (age == null)
            {
                throw new KeyNotFoundException("there is no key:\"cache-age\" in appsettings.json");
            }
            CacheType type = CacheType.Memory;
            if (cacheType == "redis")
            {
                type = CacheType.Redis;
            }
            var cacheOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(long.Parse(age))
            };
            return builder.UseMiddleware<ApiCacheMiddleware>(cacheOptions, type);
        }

        /// <summary>
        /// 使用默认配置，采用MemoryCache,缓存过期时间为2分钟
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseApiCacheMiddleware(this IApplicationBuilder builder)
        {
            var cacheOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(120)
            };
            return builder.UseMiddleware<ApiCacheMiddleware>(cacheOptions, CacheType.Memory);
        }
    }
}
