using RequestHandler;
using System.Collections;

// ReSharper disable All

namespace NetEaseMusic_API.RequestOption.Options.Playlist
{
    /// <summary>
    /// 歌单详情
    /// </summary>
    public sealed class PlaylistDetail : BaseRequestOption
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cookies">cookies</param>
        /// <param name="playlistId">歌单Id</param>
        /// <param name="collector">最近的n个收藏者</param>
        public PlaylistDetail(Hashtable cookies, int playlistId, int collector) : base(cookies)
        {
            Params.Add("id", playlistId);
            Params.Add("n", 10);
            Params.Add("s", collector);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cookies">cookies</param>
        /// <param name="playlistId">歌单Id</param>
        public PlaylistDetail(Hashtable cookies, int playlistId) : base(cookies)
        {
            Params.Add("id", playlistId);
            Params.Add("n", 100000);
        }

        public PlaylistDetail(int playlistId) : this(new Hashtable(), playlistId)
        {
        }

        public PlaylistDetail(int playlistId, int collector) : this(new Hashtable(), playlistId, collector)
        {
        }

        public override string Url => "https://music.163.com/weapi/v3/playlist/detail";

        public override CryptoType Crypto => CryptoType.Netease_linuxApi;

        public override string Ua { get; }

        public override HttpMethod HttpMethod => HttpMethod.POST;

        public override string OptionUrl { get; }
    }
}