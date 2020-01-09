using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Hosting;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using MuKai_Music.Attribute;

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

        public readonly Dictionary<string, ApiCacheAttribute> ApiMap = new Dictionary<string, ApiCacheAttribute>();

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
            var assembly = Assembly.GetExecutingAssembly();
            Type[] types = assembly.GetTypes();
            var apicontrollers = types.Where(type => type.GetCustomAttribute<ApiControllerAttribute>() != null).ToArray();
            foreach (var ctrl in apicontrollers)
            {
                MethodInfo[] controllerMethods = ctrl.GetMethods();
                var apiRoute = ctrl.GetCustomAttribute<RouteAttribute>();
                if (apiRoute == null) throw new Exception($"{ctrl.Name} doesn't have a \"Route\" Attribute");
                string ctrRoute = apiRoute.Template;
                foreach (var meth in controllerMethods)
                {
                    ApiCacheAttribute apiCacheAttribute = meth.GetCustomAttribute<ApiCacheAttribute>();
                    if (apiCacheAttribute != null)
                    {
                        string route;
                        HttpGetAttribute get = meth.GetCustomAttribute<HttpGetAttribute>();
                        HttpPostAttribute post = meth.GetCustomAttribute<HttpPostAttribute>();
                        if (get != null)
                        {
                            route = get.Template;
                        }
                        else if (post != null)
                        {
                            route = post.Template;
                        }
                        else
                        {
                            continue;
                        }
                        string key = "/" + ctrRoute + "/" + route;
                        if (this.ApiMap.ContainsKey(key))
                        {
                            throw new Exception("Api Method Route Repeat Exception！");
                        }

                        this.ApiMap.Add(key, apiCacheAttribute);
                    }
                }
            }
        }

        public async Task InvokeAsync(HttpContext httpContext,
            IMemoryCache memoryCache)
        {
            //暂存原始响应流
            var originResponseStream = httpContext.Response.Body;
            //在内存中开辟缓冲区暂存请求体
            httpContext.Request.EnableBuffering();
            using var requestReader = new StreamReader(httpContext.Request.Body);
            Endpoint endpoint = httpContext.GetEndpoint();
            string apikey = httpContext.Request.Path.Value;
            string key = await RequestHandle(requestReader, httpContext);
            if (key == null)
            {
                await _next(httpContext);
                return;
            }
            httpContext.Request.Body.Position = 0;
            if (Check(key, out string value, memoryCache, out string duration, apikey))
            {
                bool item = httpContext.Response.HasStarted;
                byte[] buffer = Encoding.UTF8.GetBytes(value);
                httpContext.Response.ContentType = "application/json; charset=utf-8";
                httpContext.Response.Headers.Add("Api-Cache", "hit");
                httpContext.Response.Headers.Add("Api-Cache-Time", duration.ToString());
                await httpContext.Response.Body.WriteAsync(buffer, 0, buffer.Length);
                return;
            }
            else
            {
                httpContext.Response.Headers.Add("Api-Cache", "miss");
                //将原本不可读取的响应流替换为可读
                using var resBody = new MemoryStream();
                httpContext.Response.Body = resBody;
                await _next(httpContext);
                using var responseReader = new StreamReader(resBody);
                resBody.Position = 0;
                await Cache(key, await ResponseHandle(responseReader, httpContext), memoryCache, apikey);
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
        /// <param name="duration"></param>
        /// <param name="apikey"></param>
        /// <returns></returns>
        private bool Check(string key, out string value, IMemoryCache memoryCache, out string duration, string apikey)
        {
            this.ApiMap.TryGetValue(apikey, out var entry);
            if (entry != null)
            {
                if (entry.NoStore)
                {
                    value = null;
                    duration = null;
                    return false;
                }
                duration = entry.Duration.ToString();
            }
            else
            {
                duration = this._options.AbsoluteExpirationRelativeToNow.Value.TotalSeconds.ToString();
            }
            return memoryCache.TryGetValue(key, out value);
        }

        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="memoryCache"></param>
        /// <param name="apikey"></param>
        /// <returns></returns>
        private async Task Cache(string key, string value, IMemoryCache memoryCache, string apikey)
        {
            await Task.Run(() =>
            {
                this.ApiMap.TryGetValue(apikey, out var entry);
                if (entry != null)
                {
                    if (entry.NoStore) return;
                    memoryCache.Set(key, value, TimeSpan.FromSeconds(entry.Duration));
                }
                else
                {
                    memoryCache.Set(key, value, _options);
                }
            });

        }
    }
}
