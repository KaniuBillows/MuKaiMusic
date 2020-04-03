using DataAbstract;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MuKai_Music.Model.Service
{
    public class MusicService
    {

        public MusicService()
        {

        }

        public async Task WirteBodyAsync(HttpContext httpContext, string content)
        {
            httpContext.Response.StatusCode = 200;
            httpContext.Response.Headers.Add(HeaderNames.ContentType, "application/json; charset=utf-8");
            byte[] buffer = Encoding.UTF8.GetBytes(content);
            await httpContext.Response.Body.WriteAsync(buffer, 0, buffer.Length);
        }

        public async Task WirteBodyAsync<T>(HttpContext httpContext, Result<T> Content)
        {
            await this.WirteBodyAsync(httpContext, JsonSerializer.Serialize(Content));
        }


    }
}
