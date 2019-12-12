using RequestHandler;
using System.Collections;

namespace NetEaseMusic_API.RequestOption.Options.Playlist
{
    /// <summary>
    /// 获取歌单热门分类
    /// </summary>
    public sealed class PlaylistHotCategories : BaseRequestOption
    {
        public PlaylistHotCategories(Hashtable cookies) : base(cookies)
        {
        }
        public PlaylistHotCategories() : this(new Hashtable())
        {
        }

        public override string Url => "https://music.163.com/weapi/playlist/hottags";

        public override CryptoType Crypto => CryptoType.Netease_weapi;

        public override string Ua { get; }

        public override HttpMethod HttpMethod => HttpMethod.POST;

        public override string OptionUrl { get; }
    }
}
