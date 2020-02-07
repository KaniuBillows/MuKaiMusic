using Microsoft.AspNetCore.Http;
using MusicApi;
using System.Collections;
using System.Threading.Tasks;

namespace MuKai_Music.Interface
{
    public interface IResultOpreate
    {
        /// <summary>
        /// 向目标服务器发送请求，将结果直接写入响应的Body中，如有cookie则返回Cookie信息
        /// </summary>
        /// <param name="httpResponse"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        Task GetResult(HttpResponse httpResponse, IRequestOption request);

        /// <summary>
        /// 将请求中的Cookie信息提取成为Hashtable
        /// </summary>
        /// <param name="httpRequest"></param>
        /// <returns></returns>
        Hashtable GetCookie(HttpRequest httpRequest);
    }
}