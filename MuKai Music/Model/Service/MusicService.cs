using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MuKai_Music.DataContext;
using NetEaseMusic_API.RequestOption.Options.Album;
using NetEaseMusic_API.RequestOption.Options.Artist;
using NetEaseMusic_API.RequestOption.Options.Banner;
using NetEaseMusic_API.RequestOption.Options.Music;
using NetEaseMusic_API.RequestOption.Options.Playlist;
using NetEaseMusic_API.RequestOption.Options.Search;
using NetEaseMusic_API.RequestOption.Options.Similar;
using RequestHandler;
using System.Threading.Tasks;

namespace MuKai_Music.Model.Service
{
    public class MusicService : ResultOperate
    {
        private readonly HttpContext httpContext;

        //private readonly MusicContext miguContext;

        public MusicService(HttpContext httpContext)
        {
            this.httpContext = httpContext;
        }

        /// <summary>
        /// 搜索
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="searchType"></param>
        /// <param name="limit"></param>
        /// <param name="offset"></param>
        public async Task<ObjectResult> Search(string keyword, SearchType searchType, int limit, int offset)
        {
            IRequestOption request = new Search(keyword, searchType, limit, offset);
            return await GetResult(httpContext.Response, request);
        }

        /// <summary>
        /// 获取歌曲的URL
        /// </summary>
        /// <param name="id"></param>
        /// <param name="br"></param>
        /// <returns></returns>
        public async Task<ObjectResult> GetMusicUrl(int id, int br)
        {
            int[] ids = { id };
            IRequestOption request = new MusicUrl(GetCookie(this.httpContext.Request), ids, br);
            return await this.GetResult(httpContext.Response, request);
        }

        /// <summary>
        /// 进行全网URL搜索
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Task<ObjectResult> SearchUrl(string key)
        {
            return null;
        }

        /// <summary>
        /// 获取歌手信息
        /// </summary>
        /// <param name="artistId">歌手Id</param>
        public async Task<ObjectResult> GetArtistDescription(int artistId)
        {
            IRequestOption request = new ArtistDescription(artistId);
            return await GetResult(httpContext.Response, request);
        }

        /// <summary>
        /// 获取歌手介绍信息
        /// </summary>
        /// <param name="artistId"></param>
        public async Task<ObjectResult> GetArtistMusics(int artistId)
        {
            IRequestOption request = new ArtistMusics(artistId);
            return await GetResult(httpContext.Response, request);
        }

        /// <summary>
        /// 获取相似歌手
        /// </summary>
        /// <param name="artistId"></param>
        public async Task<ObjectResult> GetSimilarArtist(int artistId)
        {
            IRequestOption request = new SimilarArtist(artistId);
            return await GetResult(httpContext.Response, request);
        }

        /// <summary>
        /// 获取详情专辑内容
        /// </summary>
        /// <param name="id">专辑Id</param>
        /// <returns></returns>
        public async Task<ObjectResult> GetAlbumDetail(int id)
        {
            IRequestOption request = new Album(id);
            return await GetResult(httpContext.Response, request);
        }

        /// <summary>
        /// 获取精品歌单
        /// </summary>
        /// <param name="category">种类，默认为全部</param>
        /// <param name="limit">数量</param>
        public async Task<ObjectResult> GetHighQualityPlaylist(string category, int limit)
        {
            IRequestOption request = new PlaylistHighQuality(GetCookie(httpContext.Request), category, limit);
            return await GetResult(httpContext.Response, request);
        }

        /// <summary>
        /// 获取推荐新歌
        /// </summary>
        public async Task<ObjectResult> GetPersonalizedNewMusic()
        {
            IRequestOption request = new NewMusicPersonalized(GetCookie(httpContext.Request));
            return await GetResult(httpContext.Response, request);
        }

        /// <summary>
        /// 获取推荐歌单
        /// </summary>
        /// <param name="limit"></param>
        /// <returns></returns>
        public async Task<ObjectResult> GetPersonalizedPlaylist(int limit)
        {
            IRequestOption request = new PlaylistPersonalized(GetCookie(httpContext.Request), limit);
            return await GetResult(httpContext.Response, request);
        }


        /// <summary>
        /// 获取歌曲歌词
        /// </summary>
        /// <param name="id"></param>
        public async Task<ObjectResult> GetLyric(int id)
        {
            IRequestOption request = new Lyric(id);
            return await GetResult(httpContext.Response, request);
        }

        /// <summary>
        /// 获取歌曲详情
        /// </summary>
        /// <param name="musicId"></param>
        public async Task<ObjectResult> GetMusicDetail(int[] musicId)
        {
            IRequestOption request = new MusicInfo(musicId);
            return await GetResult(httpContext.Response, request);
        }

        /// <summary>
        /// 获取歌单全部分类
        /// </summary>
        public async Task<ObjectResult> GetPlaylistCategories()
        {
            IRequestOption request = new PlaylistCategories();
            return await GetResult(httpContext.Response, request);
        }

        /// <summary>
        /// 获取热门歌单分类
        /// </summary>
        public async Task<ObjectResult> GetHotCategories()
        {
            IRequestOption request = new PlaylistHotCategories();
            return await GetResult(httpContext.Response, request);
        }

        /// <summary>
        /// 获取分类下的歌单
        /// </summary>
        /// <param name="category"></param>
        /// <param name="limit"></param>
        /// <param name="offset"></param>
        public async Task<ObjectResult> GetPlaylistInCategory(string category, int limit, int offset)
        {
            IRequestOption request = new PlaylistInCategory(category, limit, offset);
            return await GetResult(httpContext.Response, request);
        }

        /// <summary>
        /// 获取歌单详情
        /// </summary>
        /// <param name="playlistId"></param>
        public async Task<ObjectResult> GetPlaylistDetail(int playlistId)
        {
            IRequestOption request = new PlaylistDetail(playlistId);
            return await GetResult(httpContext.Response, request);
        }

        /// <summary>
        /// 获取歌曲的相似歌单
        /// </summary>
        /// <param name="musicId">音乐Id</param>
        /// <param name="limit"></param>
        /// <param name="offset"></param>
        public async Task<ObjectResult> GetSimilarPlaylist(int musicId, int limit, int offset)
        {
            IRequestOption request = new SimilarPlaylist(musicId, limit, offset);
            return await GetResult(httpContext.Response, request);
        }

        /// <summary>
        /// 获取相似单曲
        /// </summary>
        /// <param name="musicId"></param>
        /// <param name="limit"></param>
        /// <param name="offset"></param>
        public async Task<ObjectResult> GetSimilarMusics(int musicId, int limit, int offset)
        {
            IRequestOption request = new SimilarMusic(musicId, limit, offset);
            return await GetResult(httpContext.Response, request);
        }

        /// <summary>
        /// 获取日推歌曲,需要登录
        /// </summary>
        public async Task<ObjectResult> GetRecommendMusics()
        {
            IRequestOption request = new RecommendMusics(GetCookie(httpContext.Request));
            return await GetResult(httpContext.Response, request);
        }

        /// <summary>
        /// 获取日推歌单，需要登录
        /// </summary>
        public async Task<ObjectResult> GetRecommendPlaylist()
        {
            IRequestOption request = new RecommendPlaylist(GetCookie(httpContext.Request));
            return await GetResult(httpContext.Response, request);
        }

        /// <summary>
        /// 获取首页轮播图
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public async Task<ObjectResult> GetBanner(BannerType type)
        {
            IRequestOption request = new Banner(GetCookie(httpContext.Request), type);
            return await GetResult(httpContext.Response, request);
        }
    }
}
