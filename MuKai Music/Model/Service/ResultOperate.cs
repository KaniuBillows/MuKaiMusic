using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using MuKai_Music.Interface;
using RequestHandler;
using System;
using System.Collections;
using System.Text.Json;
using System.Threading.Tasks;

namespace MuKai_Music.Model.Service
{
    /// <summary>
    /// 处理请求结果
    /// </summary>
    public abstract class ResultOperate : IResultOpreate
    {

        /// <summary>
        /// 获取结果 此方法适用于，不需要返回结果进行任何处理的http调用
        /// </summary>
        /// <param name="response">真客户端响应</param>
        /// <param name="request">待请求接口信息</param>
        /// <returns></returns>
        public async Task<ObjectResult> GetResult(HttpResponse response, IRequestOption request)
        {
            var httpmessage = await request.Request();

            // await response.WriteAsync(await httpmessage.Content.ReadAsStringAsync(), Encoding.UTF8);

            var cookies = httpmessage.Headers.Contains(HeaderNames.SetCookie) ? httpmessage.Headers.GetValues(HeaderNames.SetCookie) : Array.Empty<string>();
            foreach (string cookie in cookies)
            {
                response.Headers.Add(HeaderNames.SetCookie, cookie);
            }
            return new ObjectResult(JsonSerializer.Deserialize<object>(await httpmessage.Content.ReadAsStringAsync()));
        }

        /// <summary>
        /// 获取结果，并序列化为目标类型
        /// </summary>
        /// <typeparam name="TResult">目标类型</typeparam>
        /// <param name="response">真客户端响应</param>
        /// <param name="request">待请求接口信息</param>
        /// <returns></returns>
        public async Task<TResult> GetResult<TResult>(HttpResponse response, IRequestOption request)
        {
            var httpmessage = await request.Request();
            var cookies = httpmessage.Headers.Contains(HeaderNames.SetCookie) ? httpmessage.Headers.GetValues(HeaderNames.SetCookie) : Array.Empty<string>();
            foreach (string cookie in cookies)
            {
                response.Headers.Add(HeaderNames.SetCookie, cookie);
            }
            return JsonSerializer.Deserialize<TResult>(await httpmessage.Content.ReadAsStringAsync());
        }

        /// <summary>
        /// 获取结果，并序列号目标类型
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<TResult> GetResult<TResult>(IRequestOption request)
        {
            var httpmessage = await request.Request();
            return JsonSerializer.Deserialize<TResult>(await httpmessage.Content.ReadAsStringAsync());
        }

        /// <summary>
        /// 将真客户端请求头转换为哈希表形式
        /// </summary>
        /// <param name="httpRequest"></param>
        /// <returns></returns>
        public Hashtable GetCookie(HttpRequest httpRequest)
        {
            var hs = new Hashtable();
            foreach (var cookie in httpRequest.Cookies)
            {
                hs.Add(cookie.Key, cookie.Value);
            }
            return hs;
        }
    }
}
