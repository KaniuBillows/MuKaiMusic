using System.Collections;

namespace MusicApi.NetEase.Playlist
{
    /// <summary>
    /// 推荐歌单
    /// </summary>
    public sealed class PlaylistPersonalized : BaseRequestOption
    {
        public PlaylistPersonalized(Hashtable cookies, int limit) : base(cookies)
        {
            Params.Add("limit", limit);
            Params.Add("total", true);
            Params.Add("n", 1000);
        }

        public PlaylistPersonalized(int limit) : this(new Hashtable(), limit)
        {
        }

        public PlaylistPersonalized() : this(30)
        {
        }
        public override string Url => " https://music.163.com/weapi/personalized/playlist";

        public override CryptoType Crypto => CryptoType.Netease_weapi;

        public override string Ua { get; }

        public override HttpMethod HttpMethod => HttpMethod.POST;

        public override string OptionUrl { get; }
    }
}