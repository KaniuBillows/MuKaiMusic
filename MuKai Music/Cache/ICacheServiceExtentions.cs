using System;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MuKai_Music.Middleware;

namespace MuKai_Music.Cache
{
    public static class ICacheServiceExtentions
    {
        public static IServiceCollection AddICache(this IServiceCollection services, Action<CacheOption> action)
        {
            var option = new CacheOption();
            action(option);
            if (option.CacheType == CacheType.Memory)
            {
                services.AddSingleton<ICache, MemCache>((s) =>
                {
                    return new MemCache(new MemoryCache(new MemoryCacheOptions()))
                    {
                        CacheOption = option
                    };
                });
            }
            else
            {
                services.AddSingleton<ICache, RedisClient>((s) =>
                 {
                     IConfiguration config = s.GetRequiredService<IConfiguration>();
                     var client = new RedisClient(config)
                     {
                         CacheOption = option
                     };
                     return client;
                 });
            }
            return services;
        }
    }
}
