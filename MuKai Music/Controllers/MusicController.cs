using DataAbstract;
using Microsoft.AspNetCore.Mvc;
using MuKai_Music.Attribute;
using MuKai_Music.Model.Service;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MuKai_Music.Service
{
#nullable enable

    [Route("api")]
    [ApiController]
    public class MusicController : ControllerBase
    {
        private readonly MusicService musicService;
        private readonly IHttpClientFactory httpClientFactory;

        //   private readonly IHttpContextAccessor httpContextAccessor;

        public MusicController(IHttpClientFactory httpClientFactory, MusicService musicService)
        {
            this.httpClientFactory = httpClientFactory;
            this.musicService = musicService;
        }


        /// <summary>
        /// 获取歌手介绍
        /// </summary>
        /// <param name="id"></param>
        [HttpGet("artist/description")]
        public async Task GetArtistDescription(int id) { }

        /// <summary>
        /// 获取歌手单曲
        /// </summary>
        /// <param name="id"></param>
        [HttpGet("artist/musics")]
        public async Task GetArtistMusics(int id) { }

        /// <summary>
        /// 获取专辑信息
        /// </summary>
        /// <param name="id"></param>
        [HttpGet("album/deatail")]
        public async Task GetInfo(int id) { }

        /// <summary>
        /// 推荐新歌,作为首页默认显示
        /// </summary>
        /// <returns></returns>
        [HttpGet("music/personalized")]
        public async Task PersonalizedMusic() { }

        /// <summary>
        /// 获取网易云歌曲详情信息，包含图片等信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        [HttpGet("music/pic")]
        public async Task MusicPic([Required]string id, [Required]DataSource source) => await ServiceRequest(id, source, "/pic?id=");


        /// <summary>
        /// 获取精品歌单
        /// </summary>
        /// <param name="category"></param>
        /// <param name="limit"></param>
        [HttpGet("playlist/highQuality")]
        public async Task HighQualityPlaylist(string category, int limit) { }

        /// <summary>
        /// 推荐歌单
        /// </summary>
        /// <param name="limit"></param>
        /// <returns></returns>
        [HttpGet("playlist/personalized")]
        public async Task PersonalizedPlaylist(int limit) { }

        /// <summary>
        /// 获取歌曲的URL,当Source为咪咕时，id为copyrightid，mid为普通id
        /// </summary>
        /// <returns></returns>
        [HttpGet("music/url")]
        [ApiCache(Duration = 1200)]
        public async Task MsuicUrl(
            [Required]string id,
            [Required]DataSource source,
            string? mid)
        {
            if (DataSource.Migu.Equals(source))
            {
                if (mid == null)
                {
                    this.HttpContext.Response.StatusCode = 400;
                    return;
                }
                StringBuilder builder = new StringBuilder(Startup.Configuration[ServiceInfo.MiguAPI]);
                builder.Append("/url?cid=");
                builder.Append(id);
                builder.Append("&id=");
                builder.Append(mid);
                await this.RequestAndWrite(builder.ToString());
            }
            else
            {
                await this.ServiceRequest(id, source, "/url?id=");
            }
        }

        /// <summary>
        /// 搜索歌曲信息
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet("music/search")]
        [ResponseCache(Duration = 1200)]
        [ApiCache(Duration = 1200)]
        public async Task<Result<MusicInfo[]>> SearchUrl(string key)
        {
            StringBuilder neBuilder = GetStringBuilder(DataSource.NetEase).Append("/search?keyword=").Append(key);
            Task<MusicInfo[]> neResult = ServiceRequest<MusicInfo>(neBuilder.ToString());
            StringBuilder kwBuilder = GetStringBuilder(DataSource.Kuwo).Append("/search?keyword=").Append(key);
            Task<MusicInfo[]> kwResult = ServiceRequest<MusicInfo>(kwBuilder.ToString());
            StringBuilder miguBuilder = GetStringBuilder(DataSource.Migu).Append("/search?keyword=").Append(key);
            Task<MusicInfo[]> miguResult = ServiceRequest<MusicInfo>(miguBuilder.ToString());
            MusicInfo[] neMusic = await neResult;
            MusicInfo[] kwMusic = await kwResult;
            MusicInfo[] miguMusic = await miguResult;
            MusicInfo[] res = neMusic.Concat(kwMusic).Concat(miguMusic).ToArray();
            return new Result<MusicInfo[]>(res, 200, null);
        }

        /// <summary>
        /// 获取歌词
        /// </summary>
        /// <param name="id"></param>
        /// <param name="source"></param>
        [HttpGet("music/lyric")]
        public async Task GetLyric([Required]string id, [Required]DataSource source) => await this.ServiceRequest(id, source, "/lyric?id=");


        /// <summary>
        /// 获取全部歌单分类
        /// </summary>
        [HttpGet("playlist/categories")]
        [ResponseCache(CacheProfileName = "longTime")]
        public async Task GetCategories() { }

        /// <summary>
        /// 获取热门歌单分类
        /// </summary>
        [HttpGet("playlist/hotCategories")]
        [ResponseCache(CacheProfileName = "longTime")]
        public async Task GetHotCategories() { }

        /// <summary>
        /// 获取分类下的歌单
        /// </summary>
        /// <param name="category"></param>
        /// <param name="limit"></param>
        /// <param name="offset"></param>
        [HttpGet("playlist")]
        public async Task GetPlaylistInCategory(string category, int limit, int offset) { }

        /// <summary>
        /// 获取歌单详情
        /// </summary>
        /// <param name="id"></param>
        [HttpGet("playlist/detail")]
        public async Task GetPlaylistDetail(int id) { }

        /// <summary>
        /// 获取相似歌单
        /// </summary>
        /// <param name="id">歌曲Id</param>
        /// <param name="limit"></param>
        /// <param name="offset"></param>
        [HttpGet("playlist/similar")]
        public async Task GetSimilarPlaylist(int id, int limit, int offset) { }

        /// <summary>
        /// 获取相似歌曲
        /// </summary>
        /// <param name="id">歌曲Id</param>
        /// <param name="limit"></param>
        /// <param name="offset"></param>
        [HttpGet("music/similar")]
        public async Task GetSimlarMusic(int id, int limit, int offset) { }


        /// <summary>
        /// 获取日推歌曲，需要登录网易云账号
        /// </summary>
        [HttpGet("music/recommend")]
        [ApiCache(Duration = 43200)]
        [ResponseCache(Duration = 43200)]
        public async Task GetRecommendMusic() { }



        /// <summary>
        /// 获取用户创建的歌单列表
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="limit"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        [HttpGet("netase/playlist")]
        [ApiCache(NoStore = true)]
        [ResponseCache(NoStore = true)]
        public async Task GetUserPlaylist(int userId, int limit, int offset) { }

        /* [HttpGet("music/src")]
         [ResponseCache(NoStore = true)]
         [ApiCache(NoStore = true)]
         public IActionResult GetMusic()
         {
             return Forbid();
         }*/


        private async Task RequestAndWrite(string url)
        {
            using HttpClient client = httpClientFactory.CreateClient();
            HttpResponseMessage response = await client.GetAsync(url);
            await this.musicService.WirteBodyAsync(this.HttpContext, await response.Content.ReadAsStringAsync());
        }

        private async Task ServiceRequest(string id, DataSource dataSource, string route)
        {
            StringBuilder builder;
            switch (dataSource)
            {
                case DataSource.NetEase:
                    {
                        builder = new StringBuilder(Startup.Configuration[ServiceInfo.NeAPI]);
                        builder.Append(route);
                        builder.Append(id);
                        await this.RequestAndWrite(builder.ToString());
                    }
                    break;
                case DataSource.Migu:
                    {
                        builder = new StringBuilder(Startup.Configuration[ServiceInfo.MiguAPI]);
                        builder.Append(route);
                        builder.Append(id);
                        await this.RequestAndWrite(builder.ToString());
                    }
                    break;
                case DataSource.Kuwo:
                    {
                        builder = new StringBuilder(Startup.Configuration[ServiceInfo.KuwoAPI]);
                        builder.Append(route);
                        builder.Append(id);
                        await this.RequestAndWrite(builder.ToString());
                    }
                    break;
                default:
                    {
                        await this.musicService.WirteBodyAsync(this.HttpContext, new Result<Lyric?>(null, 400, "参数不完整！"));
                        return;
                    }
            }
        }

        private async Task<T[]> ServiceRequest<T>(string url)
        {
            using HttpClient client = this.httpClientFactory.CreateClient();
            try
            {
                HttpResponseMessage response = await client.GetAsync(url);
                return JsonSerializer.Deserialize<T[]>(await response.Content.ReadAsStringAsync());
            }
            catch (Exception)
            {
                return Array.Empty<T>();
            }
        }

        private StringBuilder GetStringBuilder(DataSource dataSource)
        {
            return dataSource switch
            {
                DataSource.NetEase => new StringBuilder(Startup.Configuration[ServiceInfo.NeAPI]),
                DataSource.Migu => new StringBuilder(Startup.Configuration[ServiceInfo.MiguAPI]),
                DataSource.Kuwo => new StringBuilder(Startup.Configuration[ServiceInfo.KuwoAPI]),
                _ => new StringBuilder(),
            };
        }
    }
}