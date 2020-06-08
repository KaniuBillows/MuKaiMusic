using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using DataAbstract;
using Microsoft.AspNetCore.Mvc;
using MuKai_Music.Attribute;
using MuKai_Music.Extions.Util;
using MuKai_Music.Model.Service;

namespace MuKai_Music.Service
{
#nullable enable

    [Route("api/music")]
    [ApiController]
    public class MusicController : ControllerBase
    {
        private readonly MusicService musicService;

        //   private readonly IHttpContextAccessor httpContextAccessor;

        public MusicController(MusicService musicService)
        {
            this.musicService = musicService;
        }


        /// <summary>
        /// 获取专辑信息
        /// </summary>
        /// <param name="id"></param>
        [HttpGet("albumDeatail")]
        public async Task GetInfo(int id) { }

        /// <summary>
        /// 推荐新歌,作为首页默认显示
        /// </summary>
        /// <returns></returns>
        [HttpGet("personalized")]
        public async Task PersonalizedMusic() { }

        /// <summary>
        /// 获取歌曲的图片
        /// </summary>
        /// <param name="id"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        [HttpGet("pic")]
        public async Task MusicPic([Required] string id, [Required] DataSource source)
        {
            await this.HttpContext.WirteBodyAsync(await this.musicService.GetPic(id, source));
        }

        /// <summary>
        /// 获取歌曲的URL
        /// </summary>
        /// <returns></returns>
        [HttpGet("url")]
        [ApiCache(Duration = 1200)]
        public async Task MsuicUrl([Required] string id, [Required] DataSource source)
        {
            await this.HttpContext.WirteBodyAsync(await this.musicService.GetUrl(id, source));
        }

        /// <summary>
        /// 搜索歌曲信息
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet("search")]
        [ResponseCache(Duration = 1200)]
        [ApiCache(Duration = 1200)]
        public async Task<Result<MusicInfo[]>> SearchMusic(string key)
        {
            return Result<MusicInfo[]>.SuccessReuslt(await this.musicService.SearchMusic(key));
        }

        /// <summary>
        /// 获取歌词
        /// </summary>
        /// <param name="id">当DataSource为migu时，id为copyrightId</param>
        /// <param name="source"></param>
        [HttpGet("lyric")]
        public async Task GetLyric([Required] string id, [Required] DataSource source)
        {
            await this.HttpContext.WirteBodyAsync(await this.musicService.GetLyric(source, id));
        }

        /// <summary>
        /// 获取相似歌曲
        /// </summary>
        /// <param name="id">歌曲Id</param>
        /// <param name="limit"></param>
        /// <param name="offset"></param>
        [HttpGet("similar")]
        public async Task GetSimlarMusic(int id, int limit, int offset) { }


        /// <summary>
        /// 获取日推歌曲，需要登录网易云账号
        /// </summary>
        [HttpGet("recommend")]
        [ApiCache(Duration = 43200)]
        [ResponseCache(Duration = 43200)]
        public async Task GetRecommendMusic() { }

    }

}