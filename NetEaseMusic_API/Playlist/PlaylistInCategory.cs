using System.Collections;

namespace MusicApi.NetEase.Playlist
{
    public enum OrderType
    {
        hot,
        @new
    }

    /// <summary>
    /// 获取分类下的歌单
    /// </summary>
    public sealed class PlaylistInCategory : BaseRequestOption
    {
        public PlaylistInCategory(Hashtable cookies, string category, OrderType order, int limit, int offset) : base(cookies)
        {
            this.Params.Add("cat", category);
            this.Params.Add("order", order.ToString());
            this.Params.Add("limit", limit);
            this.Params.Add("offset", offset);
            this.Params.Add("total", true);
        }
        public PlaylistInCategory(Hashtable cookies, string category, int limit, int offset) : this(cookies, category, OrderType.hot, limit, offset)
        {

        }
        public PlaylistInCategory(string category, OrderType order, int limit, int offset) : this(new Hashtable(), category, order, limit, offset)
        {

        }
        public PlaylistInCategory(string category, int limit, int offset) : this(category, OrderType.hot, limit, offset)
        {

        }

        public override string Url => "https://music.163.com/weapi/playlist/list";

        public override CryptoType Crypto => CryptoType.Netease_weapi;

        public override string Ua { get; }

        public override HttpMethod HttpMethod => HttpMethod.POST;

        public override string OptionUrl { get; }
    }
}
