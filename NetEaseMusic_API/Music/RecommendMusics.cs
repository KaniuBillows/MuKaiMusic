using System.Collections;

namespace MusicApi.NetEase.Music
{
    /// <summary>
    /// 日推歌曲,需要登录
    /// </summary>
    public class RecommendMusics : BaseRequestOption
    {
        public RecommendMusics(Hashtable cookies) : base(cookies)
        {
            this.Params.Add("total", true);
        }

        public override string Url => "https://music.163.com/weapi/v1/discovery/recommend/songs";

        public override CryptoType Crypto => CryptoType.Netease_weapi;

        public override string Ua { get; }

        public override HttpMethod HttpMethod => HttpMethod.POST;

        public override string OptionUrl { get; }
    }
}
