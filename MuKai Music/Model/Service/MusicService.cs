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
using NetEaseMusic_API.RequestOption.Options.User;
using RequestHandler;
using System.Threading.Tasks;

namespace MuKai_Music.Model.Service
{
    public class MusicService : ResultOperate
    {
        private readonly HttpContext httpContext;

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
        public async Task Search(string keyword, SearchType searchType, int limit, int offset)
        {
            IRequestOption request = new Search(keyword, searchType, limit, offset);
            await GetResult(httpContext.Response, request);
        }

        /// <summary>
        /// 获取歌曲的URL
        /// </summary>
        /// <param name="id"></param>
        /// <param name="br"></param>
        /// <returns></returns>
        public async Task GetMusicUrl(int id, int br)
        {
            int[] ids = { id };
            IRequestOption request = new MusicUrl(GetCookie(this.httpContext.Request), ids, br);
            await this.GetResult(httpContext.Response, request);
        }

        /// <summary>
        /// 进行全网URL搜索
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Task SearchUrl(string key)
        {
            return null;
        }

        /// <summary>
        /// 获取歌手信息
        /// </summary>
        /// <param name="artistId">歌手Id</param>
        public async Task GetArtistDescription(int artistId)
        {
            IRequestOption request = new ArtistDescription(artistId);
            await GetResult(httpContext.Response, request);
        }

        /// <summary>
        /// 获取歌手介绍信息
        /// </summary>
        /// <param name="artistId"></param>
        public async Task GetArtistMusics(int artistId)
        {
            IRequestOption request = new ArtistMusics(artistId);
            await GetResult(httpContext.Response, request);
        }

        /// <summary>
        /// 获取相似歌手
        /// </summary>
        /// <param name="artistId"></param>
        public async Task GetSimilarArtist(int artistId)
        {
            IRequestOption request = new SimilarArtist(artistId);
            await GetResult(httpContext.Response, request);
        }

        /// <summary>
        /// 获取详情专辑内容
        /// </summary>
        /// <param name="id">专辑Id</param>
        /// <returns></returns>
        public async Task GetAlbumDetail(int id)
        {
            IRequestOption request = new Album(id);
            await GetResult(httpContext.Response, request);
        }

        /// <summary>
        /// 获取精品歌单
        /// </summary>
        /// <param name="category">种类，默认为全部</param>
        /// <param name="limit">数量</param>
        public async Task GetHighQualityPlaylist(string category, int limit)
        {
            IRequestOption request = new PlaylistHighQuality(GetCookie(httpContext.Request), category, limit);
            await GetResult(httpContext.Response, request);
        }

        /// <summary>
        /// 获取推荐新歌
        /// </summary>
        public async Task GetPersonalizedNewMusic()
        {
            IRequestOption request = new NewMusicPersonalized(GetCookie(httpContext.Request));
            await GetResult(httpContext.Response, request);
        }

        /// <summary>
        /// 获取推荐歌单
        /// </summary>
        /// <param name="limit"></param>
        /// <returns></returns>
        public async Task GetPersonalizedPlaylist(int limit)
        {
            IRequestOption request = new PlaylistPersonalized(GetCookie(httpContext.Request), limit);
            await GetResult(httpContext.Response, request);
        }

        /// <summary>
        /// 获取歌曲歌词
        /// </summary>
        /// <param name="id"></param>
        public async Task GetLyric(int id)
        {
            IRequestOption request = new Lyric(id);
            await GetResult(httpContext.Response, request);
        }

        /// <summary>
        /// 获取歌曲详情
        /// </summary>
        /// <param name="musicId"></param>
        public async Task GetMusicDetail(int[] musicId)
        {
            IRequestOption request = new MusicInfo(musicId);
            await GetResult(httpContext.Response, request);
        }

        /// <summary>
        /// 获取歌单全部分类
        /// </summary>
        public async Task GetPlaylistCategories()
        {
            IRequestOption request = new PlaylistCategories();
            await GetResult(httpContext.Response, request);
        }

        /// <summary>
        /// 获取热门歌单分类
        /// </summary>
        public async Task GetHotCategories()
        {
            IRequestOption request = new PlaylistHotCategories();
            await GetResult(httpContext.Response, request);
        }

        /// <summary>
        /// 获取分类下的歌单
        /// </summary>
        /// <param name="category"></param>
        /// <param name="limit"></param>
        /// <param name="offset"></param>
        public async Task GetPlaylistInCategory(string category, int limit, int offset)
        {
            IRequestOption request = new PlaylistInCategory(category, limit, offset);
            await GetResult(httpContext.Response, request);
        }

        /// <summary>
        /// 获取歌单详情
        /// </summary>
        /// <param name="playlistId"></param>
        public async Task GetPlaylistDetail(int playlistId)
        {
            IRequestOption request = new PlaylistDetail(playlistId);
            await GetResult(httpContext.Response, request);
        }

        /// <summary>
        /// 获取歌曲的相似歌单
        /// </summary>
        /// <param name="musicId">音乐Id</param>
        /// <param name="limit"></param>
        /// <param name="offset"></param>
        public async Task GetSimilarPlaylist(int musicId, int limit, int offset)
        {
            IRequestOption request = new SimilarPlaylist(musicId, limit, offset);
            await GetResult(httpContext.Response, request);
        }

        /// <summary>
        /// 获取相似单曲
        /// </summary>
        /// <param name="musicId"></param>
        /// <param name="limit"></param>
        /// <param name="offset"></param>
        public async Task GetSimilarMusics(int musicId, int limit, int offset)
        {
            IRequestOption request = new SimilarMusic(musicId, limit, offset);
            await GetResult(httpContext.Response, request);
        }

        /// <summary>
        /// 获取日推歌曲,需要登录
        /// </summary>
        public async Task GetRecommendMusics()
        {
            IRequestOption request = new RecommendMusics(GetCookie(httpContext.Request));
            await GetResult(httpContext.Response, request);
        }

        /// <summary>
        /// 获取日推歌单，需要登录
        /// </summary>
        public async Task GetRecommendPlaylist()
        {
            IRequestOption request = new RecommendPlaylist(GetCookie(httpContext.Request));
            await GetResult(httpContext.Response, request);
        }

        /// <summary>
        /// 获取首页轮播图
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public async Task GetBanner(BannerType type)
        {
            IRequestOption request = new Banner(GetCookie(httpContext.Request), type);
            await GetResult(httpContext.Response, request);
        }

        /// <summary>
        /// 获取用户歌单列表，需要登录网易云账号
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="limit"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public async Task GetUserPlaylist(int userId, int limit, int offset)
        {
            IRequestOption request = new UserPlaylist(GetCookie(httpContext.Request), userId, limit, offset);
            await GetResult(httpContext.Response, request);
        }
    }
}
