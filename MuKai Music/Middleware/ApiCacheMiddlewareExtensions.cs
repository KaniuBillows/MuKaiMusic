using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

namespace MuKai_Music.Middleware
{
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
