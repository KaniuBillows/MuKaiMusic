using System.Collections;

namespace MusicApi.NetEase.Playlist
{
    public class RecommendPlaylist : BaseRequestOption
    {
        public RecommendPlaylist(Hashtable cookies) : base(cookies)
        {
        }

        public override string Url => "https://music.163.com/weapi/v1/discovery/recommend/resource";

        public override CryptoType Crypto => CryptoType.Netease_weapi;

        public override string Ua { get; }

        public override HttpMethod HttpMethod => HttpMethod.POST;

        public override string OptionUrl { get; }
    }
}
