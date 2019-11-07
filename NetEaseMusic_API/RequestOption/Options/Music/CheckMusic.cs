using RequestHandler;
using System.Collections;

namespace NetEaseMusic_API.RequestOption.Options.Music
{
    /// <summary>
    /// 检查音乐是否可用
    /// </summary>
    public class CheckMusic : BaseRequestOption
    {
        public CheckMusic(Hashtable cookies, int[] ids, int br = 999000) : base(cookies)
        {
            Params.Add("ids", "[" + string.Join(",", ids) + "]");
            Params.Add("br", br);
        }

        public CheckMusic(int[] ids, int br = 999000) : this(new Hashtable(), ids, br) { }

        public override string Url => "https://music.163.com/weapi/song/enhance/player/url";

        public override CryptoType Crypto => CryptoType.Netease_weapi;

        public override string Ua { get; }

        public override HttpMethod HttpMethod => HttpMethod.POST;

        public override string OptionUrl { get; }
    }
}
