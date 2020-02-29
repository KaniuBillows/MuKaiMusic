using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using MuKai_Music.Middleware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
                    RedisClient.RedisClientInstence
                );
            }
            return services;
        }
    }
}
