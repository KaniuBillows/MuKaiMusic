using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using MuKai_Music.Attribute;
using MuKai_Music.Cache;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MuKai_Music.Middleware
{
    public enum CacheType
    {
        Redis,
        Memory
    }
    /*
     * 缓存中间件，针对GET请求，进行内存或Redis缓存，缓存与请求头无关
     */
    public class ApiCacheMiddleware
    {
        private readonly RequestDelegate _next;

        public readonly Dictionary<string, ApiCacheAttribute> ApiMap = new Dictionary<string, ApiCacheAttribute>();

        public ApiCacheMiddleware(RequestDelegate next)
        {
            _next = next;
            var assembly = Assembly.GetExecutingAssembly();
            Type[] types = assembly.GetTypes();
            Type[] apicontrollers = types.Where(type => type.GetCustomAttribute<ApiControllerAttribute>() != null).ToArray();
            foreach (Type ctrl in apicontrollers)
            {
                MethodInfo[] controllerMethods = ctrl.GetMethods();
                RouteAttribute apiRoute = ctrl.GetCustomAttribute<RouteAttribute>();
                if (apiRoute == null) throw new Exception($"{ctrl.Name} doesn't have a \"Route\" Attribute");
                string ctrRoute = apiRoute.Template;
                foreach (MethodInfo meth in controllerMethods)
                {
                    ApiCacheAttribute apiCacheAttribute = meth.GetCustomAttribute<ApiCacheAttribute>();
                    if (apiCacheAttribute != null)
                    {
                        string route;
                        HttpGetAttribute get = meth.GetCustomAttribute<HttpGetAttribute>();
                        if (get != null)
                        {
                            route = get.Template;
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
        }

        public async Task InvokeAsync(HttpContext httpContext, ICache cache)
        {
            if (httpContext.Request.Method != "GET")
            {
                await this._next(httpContext);
                return;
            }
            //暂存原始响应流
            Stream originResponseStream = httpContext.Response.Body;
            //在内存中开辟缓冲区暂存请求体
            httpContext.Request.EnableBuffering();
            using var requestReader = new StreamReader(httpContext.Request.Body);
            string apikey = httpContext.Request.Path.Value;
            //获取请求的key
            string key = GetRequestKey(httpContext);
            if (key == null)
            {
                await _next(httpContext);
                return;
            }
            //将请求body归位
            httpContext.Request.Body.Position = 0;
            //检查是否存在缓存,如存在缓存则读取缓存内容，并写入响应流
            if (Check(key, out string value, cache, out string duration, apikey))
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
                //异步进行缓存
                Task cacheTask = AddCache(key, await ResponseHandle(responseReader, httpContext), cache, apikey);
                resBody.Position = 0;
                await resBody.CopyToAsync(originResponseStream);
                httpContext.Response.Body = originResponseStream;
            }
        }

        /// <summary>
        /// 根据请求信息生成Key
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        private string GetRequestKey(HttpContext httpContext)
        {
            var stringBuilder = new StringBuilder(httpContext.Request.GetDisplayUrl());
            if (!stringBuilder.ToString().Contains("/api"))
            {
                return null;
            }
            //读取body内容，生成key
            return stringBuilder.ToString();
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
            return httpContext.Response.ContentType != "application/json; charset=utf-8"
                ? null : responseContent;
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
        private bool Check(string key, out string value, ICache memoryCache, out string duration, string apikey)
        {
            this.ApiMap.TryGetValue(apikey, out ApiCacheAttribute entry);
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
                duration = memoryCache.CacheOption.Age.ToString();
            }
            value = memoryCache.GetStringKey(key);
            return value != null;
        }

        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="memoryCache"></param>
        /// <param name="apikey"></param>
        /// <returns></returns>
        private async Task AddCache(string key, string value, ICache memoryCache, string apikey)
        {
            if (value == null) return;
            this.ApiMap.TryGetValue(apikey, out ApiCacheAttribute entry);
            if (entry != null)
            {
                if (entry.NoStore) return;
                await memoryCache.SetStringKeyAsync(key, value, TimeSpan.FromSeconds(entry.Duration));
            }
            else
            {
                await memoryCache.SetStringKeyAsync(key, value, TimeSpan.FromSeconds(memoryCache.CacheOption.Age));
            }
        }
    }
}
