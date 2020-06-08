using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MuKai_Music.Extions.Util
{
    public static class HttpContextExtension
    {
        public static async Task WirteBodyAsync(this HttpContext httpContext, string content)
        {
            httpContext.Response.StatusCode = 200;
            httpContext.Response.Headers.Add(HeaderNames.ContentType, "application/json;charset=utf-8");
            byte[] buffer = Encoding.UTF8.GetBytes(content);
            await httpContext.Response.Body.WriteAsync(buffer, 0, buffer.Length);
        }
    }
}
