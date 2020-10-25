using System;
using System.Net.Http;
using System.Threading.Tasks;
using DataAbstract.Playlist;
using DataAbstract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Net.Http.Headers;
using System.Text.Json;
using Kuwo_API.Result;

namespace Kuwo_API.Controllers
{
    [ApiController]
    [Route("playlist")]
    public class PlaylistController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMemoryCache _memoryCache;
        private readonly MusicService _musicService;

        public PlaylistController(IHttpClientFactory httpClientFactory, IMemoryCache memoryCache,
            MusicService musicService)
        {
            _httpClientFactory = httpClientFactory;
            _memoryCache = memoryCache;
            _musicService = musicService;
        }

        [HttpGet("daily30")]
        public async Task<Result<PlaylistInfo>> GetTodayPlaylist()
        {
            Guid gid = Guid.NewGuid();
            var url =
                $"http://www.kuwo.cn/api/www/playlist/playListInfo?pid=1082685104&pn=1&rn=30&httpsStatus=1&reqId={gid}";
            using HttpClient client = _httpClientFactory.CreateClient();
            string token;
            if (_memoryCache.TryGetValue("token", out string value))
            {
                token = value;
            }
            else
            {
                token = await _musicService.GetKuwoToken() ?? "91M13L92PH";
            }

            client.DefaultRequestHeaders.Add(HeaderNames.Cookie, $"kw_token={token}");
            client.DefaultRequestHeaders.Add("csrf", token);
            client.DefaultRequestHeaders.Add(HeaderNames.Referer, "http://www.kuwo.cn/");
            client.DefaultRequestHeaders.Add(HeaderNames.Host, "www.kuwo.cn");
            HttpResponseMessage response = await client.GetAsync(url);
            try
            {
                PlaylistResult result =
                    JsonSerializer.Deserialize<PlaylistResult>(await response.Content.ReadAsStringAsync());
                return Result<PlaylistInfo>.SuccessReuslt(result.ToProcessedData());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return Result<PlaylistInfo>.FailResult("获取失败");
            }
        }
    }
}