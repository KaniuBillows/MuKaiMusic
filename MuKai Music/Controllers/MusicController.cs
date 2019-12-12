using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MuKai_Music.Filter;
using MuKai_Music.Model.RequestEntity;
using MuKai_Music.Model.Service;
using NetEaseMusic_API.RequestOption.Options.Banner;
using NetEaseMusic_API.RequestOption.Options.Search;
using System;
using System.Threading.Tasks;

namespace MuKai_Music.Service
{
#nullable enable

    [Route("api")]
    [ApiController]
    [ServiceFilter(typeof(MyAuthorFilter))]
    public class MusicController : ControllerBase
    {
        private readonly MusicService musicService;
        public MusicController(MyAuthorFilter myAuthor)
        {
            this.musicService = new MusicService(myAuthor.HttpContext);
        }

        /// <summary>
        /// 关键词搜索
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="type"></param>
        /// <param name="limit"></param>
        /// <param name="offset"></param>
        [HttpGet("search")]
        public async Task<ObjectResult> Search(string keyword, SearchType type, int? limit, int? offset) => await musicService.Search(keyword, type, limit ?? 30, offset ?? 0);

        /// <summary>
        /// 获取歌手介绍
        /// </summary>
        /// <param name="id"></param>
        [HttpGet("artist/description")]
        public async Task<ObjectResult> GetArtistDescription(int id) => await musicService.GetArtistDescription(id);

        /// <summary>
        /// 获取歌手单曲
        /// </summary>
        /// <param name="id"></param>
        [HttpGet("artist/musics")]
        public async Task<ObjectResult> GetArtistMusics(int id) => await musicService.GetArtistMusics(id);

        /// <summary>
        /// 获取专辑信息
        /// </summary>
        /// <param name="id"></param>
        [HttpGet("album/deatail")]
        public async Task<ObjectResult> GetInfo(int id) => await musicService.GetAlbumDetail(id);

        /// <summary>
        /// 推荐新歌
        /// </summary>
        /// <returns></returns>
        [HttpGet("music/personalized")]
        [ResponseCache(Duration = 36000)]
        public async Task<ObjectResult> PersonalizedMusic() => await musicService.GetPersonalizedNewMusic();

        /// <summary>
        /// 获取精品歌单
        /// </summary>
        /// <param name="category"></param>
        /// <param name="limit"></param>
        [HttpGet("playlist/highQuality")]
        public async Task<ObjectResult> HighQualityPlaylist(string category, int limit) => await musicService.GetHighQualityPlaylist(category, limit);

        /// <summary>
        /// 推荐歌单
        /// </summary>
        /// <param name="limit"></param>
        /// <returns></returns>
        [HttpGet("playlist/personalized")]
        public async Task<ObjectResult> PersonalizedPlaylist(int limit) => await musicService.GetPersonalizedPlaylist(limit);

        /// <summary>
        /// 获取歌曲的URL
        /// </summary>
        /// <param name="id"></param>
        /// <param name="br"></param>
        /// <returns></returns>
        [HttpGet("url")]
        public async Task<ObjectResult> MsuicUrl(int id, int br)
        {
            return await musicService.GetMusicUrl(id, br);
        }
        //全网搜索歌曲Url
        /*[HttpGet("search/Url")]
        [ResponseCache(Duration = 3600)]
        public async Task<ObjectResult> SearchUrl(string keyInfo)
        {
            return await musicService.SearchUrl(keyInfo);
        }*/

        /// <summary>
        /// 获取歌词
        /// </summary>
        /// <param name="id">歌曲id</param>
        [HttpGet("lyric")]
        public async Task<ObjectResult> GetLyric(int id) => await musicService.GetLyric(id);

        /// <summary>
        /// 获取歌曲详情
        /// </summary>
        /// <param name="ids"></param>
        [HttpPost("music/detail")]
        public async Task<ObjectResult> Detail([FromBody]int[] ids) => await musicService.GetMusicDetail(ids);

        /// <summary>
        /// 获取全部歌单分类
        /// </summary>
        [HttpGet("playlist/categories")]
        public async Task<ObjectResult> GetCategories() => await musicService.GetPlaylistCategories();

        /// <summary>
        /// 获取热门歌单分类
        /// </summary>
        [HttpGet("playlist/hotCategories")]
        public async Task<ObjectResult> GetHotCategories() => await musicService.GetHotCategories();

        /// <summary>
        /// 获取分类下的歌单
        /// </summary>
        /// <param name="category"></param>
        /// <param name="limit"></param>
        /// <param name="offset"></param>
        [HttpGet("playlist")]
        public async Task<ObjectResult> GetPlaylistInCategory(string category, int limit, int offset) => await musicService.GetPlaylistInCategory(category, limit, offset);

        /// <summary>
        /// 获取歌单详情
        /// </summary>
        /// <param name="id"></param>
        [HttpGet("playlist/detail")]
        public async Task<ObjectResult> GetPlaylistDetail(int id) => await musicService.GetPlaylistDetail(id);

        /// <summary>
        /// 获取相似歌单
        /// </summary>
        /// <param name="id">歌曲Id</param>
        /// <param name="limit"></param>
        /// <param name="offset"></param>
        [HttpGet("playlist/similar")]
        public async Task<ObjectResult> GetSimilarPlaylist(int id, int limit, int offset) => await musicService.GetSimilarPlaylist(id, limit, offset);

        /// <summary>
        /// 获取相似歌曲
        /// </summary>
        /// <param name="id">歌曲Id</param>
        /// <param name="limit"></param>
        /// <param name="offset"></param>
        [HttpGet("music/similar")]
        public async Task<ObjectResult> GetSimlarMusic(int id, int limit, int offset) => await musicService.GetSimilarMusics(id, limit, offset);

        /// <summary>
        /// 获取轮播图
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public async Task<ObjectResult> GetBanner(BannerType type) => await musicService.GetBanner(type);

        /// <summary>
        /// 获取日推歌曲，需要登录网易云账号
        /// </summary>
        [HttpGet("music/recommend")]
        public async Task<ObjectResult> GetRecommendMusic() => await this.musicService.GetRecommendMusics();

        /// <summary>
        /// 获取日推歌单，需要登录网易云账号
        /// </summary>
        [HttpGet("playlist/recommend")]
        public async Task<ObjectResult> GetRecommendPlaylist() => await this.musicService.GetRecommendMusics();
    }
}