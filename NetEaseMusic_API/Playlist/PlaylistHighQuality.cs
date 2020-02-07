using System.Collections;

namespace MusicApi.NetEase.Playlist
{
    /// <summary>
    /// 精品歌单
    /// </summary>
    public class PlaylistHighQuality : BaseRequestOption
    {
        public PlaylistHighQuality(Hashtable cookies, string category, int limit) : base(cookies)
        {
            this.Params.Add("cat", category);
            this.Params.Add("limit", limit);
            this.Params.Add("lasttime", 0);
            this.Params.Add("total", true);
        }

        public PlaylistHighQuality(string category, int limit) : this(new Hashtable(), category, limit)
        {
        }

        public override string Url => "https://music.163.com/weapi/playlist/highquality/list";

        public override CryptoType Crypto => CryptoType.Netease_weapi;

        public override string Ua { get; }

        public override HttpMethod HttpMethod => HttpMethod.POST;

        public override string OptionUrl { get; }
    }
}
