using RequestHandler;
using System.Collections;

namespace NetEaseMusic_API.RequestOption.Options.Playlist
{
    /// <summary>
    /// 获取歌单全部分类
    /// </summary>
    public sealed class PlaylistCategories : BaseRequestOption
    {
        public PlaylistCategories(Hashtable cookies) : base(cookies)
        {
        }
        public PlaylistCategories() : this(new Hashtable())
        {

        }

        public override string Url => "https://music.163.com/weapi/playlist/catalogue";

        public override CryptoType Crypto => CryptoType.Netease_weapi;

        public override string Ua { get; }

        public override HttpMethod HttpMethod => HttpMethod.POST;

        public override string OptionUrl { get; }
    }
}
