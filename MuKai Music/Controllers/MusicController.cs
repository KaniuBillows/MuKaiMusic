using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MuKai_Music.Attribute;
using MuKai_Music.Model.Service;
using NetEaseMusic_API.RequestOption.Options.Banner;
using NetEaseMusic_API.RequestOption.Options.Search;
using System.Threading.Tasks;

namespace MuKai_Music.Service
{
#nullable enable

    [Route("api")]
    [ApiController]
    public class MusicController : ControllerBase
    {
        private readonly MusicService musicService;
        private readonly IHttpContextAccessor httpContextAccessor;

        public MusicController(IHttpContextAccessor httpContextAccessor)
        {
            this.musicService = new MusicService(httpContextAccessor.HttpContext);
            this.httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// 关键词搜索
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="type"></param>
        /// <param name="limit"></param>
        /// <param name="offset"></param>
        [HttpGet("search")]
        [ApiCache(Duration = 3600)]
        public async Task Search(string keyword, SearchType type, int? limit, int? offset) => await musicService.Search(keyword, type, limit ?? 30, offset ?? 0);

        /// <summary>
        /// 获取歌手介绍
        /// </summary>
        /// <param name="id"></param>
        [HttpGet("artist/description")]
        public async Task GetArtistDescription(int id) => await musicService.GetArtistDescription(id);

        /// <summary>
        /// 获取歌手单曲
        /// </summary>
        /// <param name="id"></param>
        [HttpGet("artist/musics")]
        public async Task GetArtistMusics(int id) => await musicService.GetArtistMusics(id);

        /// <summary>
        /// 获取专辑信息
        /// </summary>
        /// <param name="id"></param>
        [HttpGet("album/deatail")]
        public async Task GetInfo(int id) => await musicService.GetAlbumDetail(id);

        /// <summary>
        /// 推荐新歌
        /// </summary>
        /// <returns></returns>
        [HttpGet("music/personalized")]
        public async Task PersonalizedMusic() => await musicService.GetPersonalizedNewMusic();

        /// <summary>
        /// 获取精品歌单
        /// </summary>
        /// <param name="category"></param>
        /// <param name="limit"></param>
        [HttpGet("playlist/highQuality")]
        public async Task HighQualityPlaylist(string category, int limit) => await musicService.GetHighQualityPlaylist(category, limit);

        /// <summary>
        /// 推荐歌单
        /// </summary>
        /// <param name="limit"></param>
        /// <returns></returns>
        [HttpGet("playlist/personalized")]
        public async Task PersonalizedPlaylist(int limit) => await musicService.GetPersonalizedPlaylist(limit);

        /// <summary>
        /// 获取歌曲的URL
        /// </summary>
        /// <param name="id"></param>
        /// <param name="br"></param>
        /// <returns></returns>
        [HttpGet("url")]
        [ApiCache(Duration = 600)]
        public async Task MsuicUrl(int id, int br) => await musicService.GetMusicUrl(id, br);

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
        public async Task GetLyric(int id) => await musicService.GetLyric(id);

        /// <summary>
        /// 获取歌曲详情
        /// </summary>
        /// <param name="ids"></param>
        [HttpPost("music/detail")]
        public async Task Detail([FromBody]int[] ids) => await musicService.GetMusicDetail(ids);

        /// <summary>
        /// 获取全部歌单分类
        /// </summary>
        [HttpGet("playlist/categories")]
        [ResponseCache(CacheProfileName = "longTime")]
        public async Task GetCategories() => await musicService.GetPlaylistCategories();

        /// <summary>
        /// 获取热门歌单分类
        /// </summary>
        [HttpGet("playlist/hotCategories")]
        [ResponseCache(CacheProfileName = "longTime")]
        public async Task GetHotCategories() => await musicService.GetHotCategories();

        /// <summary>
        /// 获取分类下的歌单
        /// </summary>
        /// <param name="category"></param>
        /// <param name="limit"></param>
        /// <param name="offset"></param>
        [HttpGet("playlist")]
        public async Task GetPlaylistInCategory(string category, int limit, int offset) => await musicService.GetPlaylistInCategory(category, limit, offset);

        /// <summary>
        /// 获取歌单详情
        /// </summary>
        /// <param name="id"></param>
        [HttpGet("playlist/detail")]
        public async Task GetPlaylistDetail(int id) => await musicService.GetPlaylistDetail(id);

        /// <summary>
        /// 获取相似歌单
        /// </summary>
        /// <param name="id">歌曲Id</param>
        /// <param name="limit"></param>
        /// <param name="offset"></param>
        [HttpGet("playlist/similar")]
        public async Task GetSimilarPlaylist(int id, int limit, int offset) => await musicService.GetSimilarPlaylist(id, limit, offset);

        /// <summary>
        /// 获取相似歌曲
        /// </summary>
        /// <param name="id">歌曲Id</param>
        /// <param name="limit"></param>
        /// <param name="offset"></param>
        [HttpGet("music/similar")]
        public async Task GetSimlarMusic(int id, int limit, int offset) => await musicService.GetSimilarMusics(id, limit, offset);

        /// <summary>
        /// 获取轮播图
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        [HttpGet("banner")]
        [ApiCache(Duration = 3600)]
        [ResponseCache(CacheProfileName = "default")]
        public async Task GetBanner(BannerType type) => await musicService.GetBanner(type);

        /// <summary>
        /// 获取日推歌曲，需要登录网易云账号
        /// </summary>
        [HttpGet("music/recommend")]
        [ApiCache(Duration = 43200)]
        [ResponseCache(Duration = 43200)]
        public async Task GetRecommendMusic() => await this.musicService.GetRecommendMusics();

        /// <summary>
        /// 获取日推歌单，需要登录网易云账号
        /// </summary>
        [HttpGet("playlist/recommend")]
        [ApiCache(Duration = 43200)]
        [ResponseCache(Duration = 43200)]
        public async Task GetRecommendPlaylist() => await this.musicService.GetRecommendMusics();

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
        public async Task GetUserPlaylist(int userId, int limit, int offset) => await musicService.GetUserPlaylist(userId, limit, offset);
    }
}