using Microsoft.AspNetCore.Builder;

namespace MuKai_Music.Middleware.ApiCache
{
    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class ApiCacheMiddlewareExtensions
    {

        /// <summary>
        /// 读取配置文件进行缓存设置
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseApiCacheMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ApiCacheMiddleware>();
        }
    }
}
