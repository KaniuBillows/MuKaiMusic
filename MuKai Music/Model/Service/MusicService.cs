using Microsoft.AspNetCore.Http;
using MuKai_Music.Model.ResponseEntity;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using MuKai_Music.Model.ResponseEntity.MusicUrlResult;
using MuKai_Music.Model.ResponseEntity.MusicUrlResult.Kuwo;
using MuKai_Music.Model.ResponseEntity.MusicUrlResult.NetEase;
using MuKai_Music.Model.ResponseEntity.SearchResult;
using MuKai_Music.Model.ResponseEntity.SearchResult.Kuwo;
using MuKai_Music.Model.ResponseEntity.SearchResult.NetEase;
using MusicApi.NetEase.Search;
using MusicApi.NetEase.Music;
using MusicApi.NetEase.Similar;
using MusicApi.NetEase.Playlist;
using MusicApi.NetEase.Artist;
using MusicApi.NetEase.Banner;
using MusicApi.Kuwo.Search;
using MusicApi;
using MusicApi.Migu.Music;
using MuKai_Music.Model.ResponseEntity.MusicUrlResult.Migu;
using MuKai_Music.Model.ResponseEntity.SearchResult.Migu;
using MusicApi.Migu.Search;
using MuKai_Music.Model.RequestEntity.Music;
using MuKai_Music.Model.ResponseEntity.PersonlizedResult;
using MuKai_Music.Model.ResponseEntity.LyricResult;
using MiGu_Music_API.Music;
using MusicApi.Kuwo.Music;

namespace MuKai_Music.Model.Service
{
    public class MusicService : ResultOperate
    {
        private readonly HttpContext httpContext;
        protected override IHttpClientFactory HttpClientFactory { get; set; }

        public MusicService(HttpContext httpContext,
            IHttpClientFactory httpClientFactory)
        {
            this.httpContext = httpContext;
            this.HttpClientFactory = httpClientFactory;
        }



        /// <summary>
        /// 获取歌曲的URL
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public async Task<IResult<MusicUrlInfo>> GetMusicUrl(Music_Param param)
        {
            BaseResult<MusicUrlInfo> res = null;
            IRequestOption req;
            switch (param.DataSource)
            {
                case DataSource.Kuwo:
                    {
                        if (!param.KuwoId.HasValue) break;
                        req = new MusicApi.Kuwo.Music.Music_Url(param.KuwoId.Value);
                        KuwoUrl_Result kuwoResult = await GetResult<KuwoUrl_Result>(req);
                        res = new BaseResult<MusicUrlInfo>(kuwoResult.ToProcessedData(), 200, null);
                    }
                    break;
                case DataSource.NetEase:
                    {
                        if (!param.NeteaseId.HasValue) break;
                        req = new MusicApi.NetEase.Music.Music_Url(param.NeteaseId.Value);
                        NetEaseUrl_Result netEaseResult = await GetResult<NetEaseUrl_Result>(req);
                        res = new BaseResult<MusicUrlInfo>(netEaseResult.ToProcessedData(), 200, null);
                    }
                    break;
                case DataSource.Migu:
                    {
                        if (param.MiguId == null) break;
                        req = new Web_Listen_Url(param.MiguId);
                        MiguUrl_Result miguResult = await GetResult<MiguUrl_Result>(req);
                        res = new BaseResult<MusicUrlInfo>(miguResult.ToProcessedData(), 200, null);
                    }
                    break;
                default: break;
            }
            return res ?? new BaseResult<MusicUrlInfo>(null, 400, "参数错误!");
        }

        /// <summary>
        /// 进行全网歌曲搜索,暂时只支持搜索歌曲
        /// </summary>
        /// <param name="key"></param>
        /// <param name="kuwoToken"></param>
        /// <returns></returns>
        public async Task<IResult<DataEntity.MusicInfo[]>> SearchMusic(string key, string kuwoToken)
        {
            Kuwo_Search_Result kuwoResult = await GetResult<Kuwo_Search_Result>(new Music_Search(kuwoToken, key, 10, 0));
            NetEase_Search_Result netEaseResult = await GetResult<NetEase_Search_Result>(new Search(new System.Collections.Hashtable(), key, SearchType.Song, 10, 0));
            Migu_Search_Result miguResult = await GetResult<Migu_Search_Result>(new Web_Search(key));
            DataEntity.MusicInfo[] kuwo = kuwoResult.ToProcessedData();
            DataEntity.MusicInfo[] netease = netEaseResult.ToProcessedData();
            DataEntity.MusicInfo[] migu = miguResult.ToProcessedData();
            int length = kuwo.Length + netease.Length + migu.Length;
            DataEntity.MusicInfo[] res = new DataEntity.MusicInfo[length];
            //合并结果 1.网易，2.酷我，3.咪咕 
            for (int i = 0, x = 0, y = 0, w = 0; i < length;)
            {
                if (x < netease.Length && y < kuwo.Length && w < migu.Length)
                {
                    res[i++] = netease[x++];
                    res[i++] = kuwo[y++];
                    res[i++] = migu[w++];
                }
                else if (!(x < netease.Length))
                {
                    if (y < kuwo.Length && w < migu.Length)
                    {
                        res[i++] = kuwo[y++];
                        res[i++] = migu[w++];
                    }
                    else if (y < kuwo.Length)
                    {
                        res[i++] = kuwo[y++];
                    }
                    else if (w < migu.Length)
                    {
                        res[i++] = kuwo[w++];
                    }
                }
                else if (!(y < kuwo.Length))
                {
                    if (x < netease.Length && w < migu.Length)
                    {
                        res[i++] = netease[x++];
                        res[i++] = migu[w++];
                    }
                    else if (x < netease.Length)
                    {
                        res[i++] = netease[x++];
                    }
                    else if (w < migu.Length)
                    {
                        res[i++] = migu[w++];
                    }
                }
                else if (!(w < migu.Length))
                {
                    if (x < netease.Length && y < kuwo.Length)
                    {
                        res[i++] = netease[x++];
                        res[i++] = kuwo[y++];
                    }
                    else if (x < netease.Length)
                    {
                        res[i++] = netease[x++];
                    }
                    else if (y < kuwo.Length)
                    {
                        res[i++] = netease[y++];
                    }
                }
            }
            return new BaseResult<DataEntity.MusicInfo[]>(res, 200, null);
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
            IRequestOption request = new MusicApi.NetEase.Album.Album(id);
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
        public async Task<IResult<DataEntity.MusicInfo[]>> GetPersonalizedNewMusic()
        {
            IRequestOption request = new NewMusicPersonalized(GetCookie(httpContext.Request));
            Netease_PersonlizedResult res = await GetResult<Netease_PersonlizedResult>(request);
            return new BaseResult<DataEntity.MusicInfo[]>(res.ToProcessedData(), 200, null);
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
        /// <param name="param"></param>
        /// <returns></returns>
        public async Task<IResult<ResponseEntity.LyricResult.Lyric[]>> GetLyric(Music_Param param)
        {
            IRequestOption request;
            ResponseEntity.LyricResult.Lyric[] lyricInfo = null;
            switch (param.DataSource)
            {
                case DataSource.NetEase:
                    {
                        if (!param.NeteaseId.HasValue) break;
                        request = new MusicApi.NetEase.Music.Lyric(param.NeteaseId.Value);
                        lyricInfo = (await GetResult<NetEase_Lyric_Result>(request)).ToProcessedData();

                    }
                    break;
                case DataSource.Migu:
                    {
                        request = new Web_Lyric(param.MiguId);
                        lyricInfo = (await GetResult<Migu_Lyric_Result>(request)).ToProcessedData();
                    }
                    break;
                case DataSource.Kuwo:
                    {
                        if (!param.KuwoId.HasValue) break;
                        request = new Music_Lyric(param.KuwoId.Value);
                        lyricInfo = (await GetResult<Kuwo_Lyric_Result>(request)).ToProcessedData();
                    }
                    break;
                default:
                    break;
            }
            return lyricInfo == null ?
                new BaseResult<ResponseEntity.LyricResult.Lyric[]>(null, 400, "参数不完整！")
                : new BaseResult<ResponseEntity.LyricResult.Lyric[]>(lyricInfo, 200, null);
        }

        /// <summary>
        /// 获取歌曲详情
        /// </summary>
        /// <param name="musicId"></param>
        public async Task GetMusicDetail(int musicId)
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

        /// <summary>
        /// 获取酷我token，如果获取失败，这返回null
        /// </summary>
        /// <returns></returns>
        public async Task<IResult<string>> GetKuwoToken()
        {
            using HttpClient client = this.HttpClientFactory.CreateClient();
            HttpResponseMessage response = await client.GetAsync("http://www.kuwo.cn");
            if (response.Headers.TryGetValues("Set-Cookie", out IEnumerable<string> values))
            {
                IEnumerator<string> enumerator = values.GetEnumerator();
                enumerator.MoveNext();
                string value = enumerator.Current;
                return new BaseResult<string>(value.Substring(9, 11), 200, null);
            }
            else
            {
                return new BaseResult<string>(null, 500, "获取token失败!");
            }
        }

        /*/// <summary>
        /// 搜索酷我曲库
        /// </summary>
        /// <param name="token"></param>
        /// <param name="keyWord"></param>
        /// <returns></returns>
        public async Task KuwoSearch(string token, string keyWord)
        {
            IRequestOption request = new Music_Search(token, keyWord);
            await GetResult(httpContext.Response, request);
        }*/


    }
}
