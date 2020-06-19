using DataAbstract;
using DataAbstract.Playlist;
using Microsoft.AspNetCore.Mvc;
using MusicApi.NetEase.Music;
using MusicApi.NetEase.Playlist;
using MusicApi.NetEase.Search;
using Netease_API.Results;
using Netease_API.Results.Music;
using Netease_API.Results.Playlist;
using Netease_API.Service;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Netease_API.Controllers
{
    [ApiController]
    [Route("/")]
    public class NetEaseController : ControllerBase
    {
        private readonly MusicService musicService;

        public NetEaseController(MusicService musicService)
        {
            this.musicService = musicService;
        }


        [HttpGet("search")]
        public async Task<List<MusicInfo>> Search(string keyword, int pageNo = 1, int limit = 10)
        {
            pageNo = pageNo - 1 < 0 ? 0 : pageNo - 1;
            Search search = new Search(new Hashtable(), keyword, SearchType.Song, limit, pageNo);
            HttpResponseMessage response = await search.Request();
            try
            {
                List<MusicInfo> res = JsonSerializer.Deserialize<MusicSearchResult>(await response.Content.ReadAsStringAsync()).ToProcessedData().ToList();
                return await this.musicService.MuiscsProcess(res);
            }
            catch (Exception)
            {
                return new List<MusicInfo>();
            }
        }

        /// <summary>
        /// 获取歌曲的详情信息,不包含Url
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("detail/music")]
        public async Task<Result<MusicInfo>> MusicDetail(int id)
        {
            MusicDetail req = new MusicDetail(id);
            try
            {
                var response = await req.Request();
                var res = JsonSerializer.Deserialize<SongDetailResult>(await response.Content.ReadAsStringAsync());
                return Result<MusicInfo>.SuccessReuslt(res.ToProcessedData().FirstOrDefault());
            }
            catch (Exception)
            {
                return Result<MusicInfo>.FailResult("服务出错了");
            }
        }

        [HttpGet("lyric")]
        public async Task<Result<Lyric[]>> GetLyirc(int id)
        {
            GetLyric request = new GetLyric(id);
            HttpResponseMessage response = await request.Request();
            try
            {
                return Result<Lyric[]>.SuccessReuslt(JsonSerializer.Deserialize<LyricResult>(await response.Content.ReadAsStringAsync()).ToProcessedData());
            }
            catch (Exception)
            {
                return Result<Lyric[]>.FailResult("获取失败");
            }
        }

        [HttpGet("url")]
        public async Task<Result<string>> GetUrl(int id)
        {
            Music_Url request = new Music_Url(id);
            HttpResponseMessage res = await request.Request();
            try
            {
                return Result<string>.SuccessReuslt(JsonSerializer.Deserialize<NetEaseUrl_Result>(await res.Content.ReadAsStringAsync()).Data[0].Url);
            }
            catch (Exception)
            {
                return Result<string>.FailResult("无法获取");
            }
        }

        [HttpGet("pic")]
        public async Task<Result<string>> GetPic(int id)
        {
            MusicDetail request = new MusicDetail(id);
            HttpResponseMessage response = await request.Request();
            try
            {
                return Result<string>.SuccessReuslt(JsonSerializer.Deserialize<PicResult>(await response.Content.ReadAsStringAsync()).Songs[0].Album.PicUrl);
            }
            catch (Exception)
            {
                return Result<string>.FailResult("获取失败");
            }
        }
    }
}
