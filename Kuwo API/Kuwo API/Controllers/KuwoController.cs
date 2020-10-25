using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using DataAbstract;
using DataAbstract.Playlist;
using Kuwo_API.Result;
using Kuwo_API.Result.Lyric;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Net.Http.Headers;

namespace Kuwo_API.Controllers
{
    [ApiController]
    [Route("/")]
    public class KuwoController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMemoryCache _memoryCache;
        private readonly MusicService _musicService;

        public KuwoController(IHttpClientFactory httpClientFactory,
            IMemoryCache memoryCache, MusicService musicService)
        {
            this._httpClientFactory = httpClientFactory;
            this._memoryCache = memoryCache;
            _musicService = musicService;
        }

        [HttpGet("search")]
        public async Task<MusicInfo[]> Search(string keyword, int pageNo = 1, int limit = 10)
        {
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

            var url =
                $"http://www.kuwo.cn/api/www/search/searchMusicBykeyWord?key={Uri.EscapeDataString(keyword)}&pn={pageNo}&rn={limit}&reqId={Guid.NewGuid()}";
            client.DefaultRequestHeaders.Add(HeaderNames.Cookie, $"kw_token={token}");
            client.DefaultRequestHeaders.Add("csrf", token);
            client.DefaultRequestHeaders.Add(HeaderNames.Referer, "http://www.kuwo.cn/");
            client.DefaultRequestHeaders.Add(HeaderNames.Host, "www.kuwo.cn");
            HttpResponseMessage response = await client.GetAsync(url);
            try
            {
                SearchResult result =
                    JsonSerializer.Deserialize<SearchResult>(await response.Content.ReadAsStringAsync());
                return result.ToProcessedData();
            }
            catch (Exception)
            {
                return Array.Empty<MusicInfo>();
            }
        }

        [HttpGet("url")]
        public async Task<Result<string>> UrlInfo(int id)
        {
            Guid gid = Guid.NewGuid();
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            var t = Convert.ToInt64(ts.TotalMilliseconds);
            var url =
                $"http://www.kuwo.cn/url?format=mp3&rid={id}&response=url&type=convert_url3&from=web&t={t}&reqId={gid}";
            using HttpClient client = this._httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add(HeaderNames.Referer, "http://www.kuwo.cn/");
            client.DefaultRequestHeaders.Add(HeaderNames.Host, "www.kuwo.cn");
            HttpResponseMessage response = await client.GetAsync(url);
            try
            {
                KuwoUrl_Result result =
                    JsonSerializer.Deserialize<KuwoUrl_Result>(await response.Content.ReadAsStringAsync());
                return Result<string>.SuccessReuslt(result.Url);
            }
            catch (Exception)
            {
                return Result<string>.FailResult("无法获取");
            }
        }

        [HttpGet("lyric")]
        public async Task<Result<Lyric[]>> Lyric(int id)
        {
            Guid gid = Guid.NewGuid();
            var url = $"http://m.kuwo.cn/newh5/singles/songinfoandlrc?musicId={id}&&reqId={gid}";
            using HttpClient client = this._httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add(HeaderNames.Referer, "http://www.kuwo.cn/");
            client.DefaultRequestHeaders.Add(HeaderNames.Host, "www.kuwo.cn");
            HttpResponseMessage response = await client.GetAsync(url);
            try
            {
                LyricResult result =
                    JsonSerializer.Deserialize<LyricResult>(await response.Content.ReadAsStringAsync());
                return Result<Lyric[]>.SuccessReuslt(result.ToProcessedData());
            }
            catch (Exception)
            {
                return Result<Lyric[]>.FailResult("获取失败");
            }
        }

        [HttpGet("pic")]
        public async Task<Result<string>> GetPic(int id)
        {
            Guid gid = Guid.NewGuid();
            var url = $"http://www.kuwo.cn/api/www/music/musicInfo?mid={id}&reqId={gid}";
            using HttpClient client = this._httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add(HeaderNames.Referer, "http://www.kuwo.cn/");
            client.DefaultRequestHeaders.Add(HeaderNames.Host, "www.kuwo.cn");
            HttpResponseMessage response = await client.GetAsync(url);
            try
            {
                PicResult result = JsonSerializer.Deserialize<PicResult>(await response.Content.ReadAsStringAsync());
                return Result<string>.SuccessReuslt(result.Data.PicUrl.Replace("http", "https"));
            }
            catch (Exception)
            {
                return Result<string>.FailResult("获取失败");
            }
        }


        [HttpGet("search_hotkey")]
        public async Task<Result<List<string>>> GetSearchHotkey()
        {
            Guid gid = Guid.NewGuid();
            var url = $"http://www.kuwo.cn/api/www/search/searchKey?key=&httpsStatus=1&reqId={gid}";
            string token;
            if (_memoryCache.TryGetValue("token", out string value))
            {
                token = value;
            }
            else
            {
                token = await _musicService.GetKuwoToken() ?? "91M13L92PH";
            }

            using HttpClient client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add(HeaderNames.Cookie, $"kw_token={token}");
            client.DefaultRequestHeaders.Add("csrf", token);
            client.DefaultRequestHeaders.Add(HeaderNames.Referer, "http://www.kuwo.cn/");
            client.DefaultRequestHeaders.Add(HeaderNames.Host, "www.kuwo.cn");
            HttpResponseMessage response = await client.GetAsync(url);
            try
            {
                return Result<List<string>>.SuccessReuslt(JsonSerializer
                    .Deserialize<SearchHotkeyResult>(await response.Content.ReadAsStringAsync()).Data);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}