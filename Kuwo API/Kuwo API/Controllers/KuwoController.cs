using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using DataAbstract;
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

        public KuwoController(IHttpClientFactory httpClientFactory,
            IMemoryCache memoryCache)
        {
            this._httpClientFactory = httpClientFactory;
            this._memoryCache = memoryCache;
        }

        private async Task<string> GetKuwoToken()
        {
            using HttpClient client = this._httpClientFactory.CreateClient();
            HttpResponseMessage response = await client.GetAsync("http://www.kuwo.cn");
            if (response.Headers.TryGetValues("Set-Cookie", out IEnumerable<string> values))
            {
                IEnumerator<string> enumerator = values.GetEnumerator();
                enumerator.MoveNext();
                string value = enumerator.Current;
                value = value.Substring(9, 10);
                this._memoryCache.Set("token", value);
                return value;
            }
            else
            {
                return null;
            }
        }

        [HttpGet("search")]
        public async Task<MusicInfo[]> Search(string keyword, int pageNo = 1, int limit = 10)
        {
            using HttpClient client = this._httpClientFactory.CreateClient();
            if (this._memoryCache.TryGetValue("token", out string token))
            {

                string Url = $"http://www.kuwo.cn/api/www/search/searchMusicBykeyWord?key={Uri.EscapeDataString(keyword)}&pn={pageNo}&rn={limit}&reqId={Guid.NewGuid()}";

                client.DefaultRequestHeaders.Add(HeaderNames.Cookie, $"kw_token={token}");
                client.DefaultRequestHeaders.Add("csrf", token);
                client.DefaultRequestHeaders.Add(HeaderNames.Referer, "http://www.kuwo.cn/");
                client.DefaultRequestHeaders.Add(HeaderNames.Host, "www.kuwo.cn");
                HttpResponseMessage response = await client.GetAsync(Url);
                try
                {
                    SearchResult result = JsonSerializer.Deserialize<SearchResult>(await response.Content.ReadAsStringAsync());
                    return result.ToProcessedData();
                }
                catch (Exception)
                {
                    return Array.Empty<MusicInfo>();
                }
            }
            else
            {
                Task<string> tokenTask = this.GetKuwoToken();
                string Url = $"http://www.kuwo.cn/api/www/search/searchMusicBykeyWord?key={Uri.EscapeDataString(keyword)}&pn={pageNo}&rn={limit}&reqId={Guid.NewGuid()}";
                client.DefaultRequestHeaders.Add(HeaderNames.Referer, "http://www.kuwo.cn/");
                client.DefaultRequestHeaders.Add(HeaderNames.Host, "www.kuwo.cn");
                token = await tokenTask ?? "91M13L92PH";
                client.DefaultRequestHeaders.Add(HeaderNames.Cookie, $"kw_token={token}");
                client.DefaultRequestHeaders.Add("csrf", token);
                HttpResponseMessage response = await client.GetAsync(Url);
                try
                {
                    SearchResult result = JsonSerializer.Deserialize<SearchResult>(await response.Content.ReadAsStringAsync());
                    return result.ToProcessedData();
                }
                catch (Exception)
                {
                    return Array.Empty<MusicInfo>();
                }
            }
        }

        [HttpGet("url")]
        public async Task<Result<string>> UrlInfo(int id)
        {
            var gid = Guid.NewGuid();
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            long t = Convert.ToInt64(ts.TotalMilliseconds);
            string Url = $"http://www.kuwo.cn/url?format=mp3&rid={id}&response=url&type=convert_url3&from=web&t={t}&reqId={gid}";
            using HttpClient client = this._httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add(HeaderNames.Referer, "http://www.kuwo.cn/");
            client.DefaultRequestHeaders.Add(HeaderNames.Host, "www.kuwo.cn");
            HttpResponseMessage response = await client.GetAsync(Url);
            try
            {
                KuwoUrl_Result result = JsonSerializer.Deserialize<KuwoUrl_Result>(await response.Content.ReadAsStringAsync());
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
            var gid = Guid.NewGuid();
            string Url = $"http://m.kuwo.cn/newh5/singles/songinfoandlrc?musicId={id}&&reqId={gid}";
            using HttpClient client = this._httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add(HeaderNames.Referer, "http://www.kuwo.cn/");
            client.DefaultRequestHeaders.Add(HeaderNames.Host, "www.kuwo.cn");
            HttpResponseMessage response = await client.GetAsync(Url);
            try
            {
                LyricResult result = JsonSerializer.Deserialize<LyricResult>(await response.Content.ReadAsStringAsync());
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
            var gid = Guid.NewGuid();
            string Url = $"http://www.kuwo.cn/api/www/music/musicInfo?mid={id}&reqId={gid}";
            using HttpClient client = this._httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add(HeaderNames.Referer, "http://www.kuwo.cn/");
            client.DefaultRequestHeaders.Add(HeaderNames.Host, "www.kuwo.cn");
            HttpResponseMessage response = await client.GetAsync(Url);
            try
            {
                PicResult result = JsonSerializer.Deserialize<PicResult>(await response.Content.ReadAsStringAsync());
                return Result<string>.SuccessReuslt(result.Data.PicUrl);
            }
            catch (Exception)
            {
                return Result<string>.SuccessReuslt("获取失败");
            }
        }
    }
}
