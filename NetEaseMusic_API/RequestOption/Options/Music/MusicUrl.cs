using RequestHandler;
using System.Collections;

namespace NetEaseMusic_API.RequestOption.Options.Music
{
    /// <summary>
    /// 获取歌曲Url
    /// </summary>
    public class MusicUrl : BaseRequestOption
    {
        public MusicUrl(Hashtable cookies, int[] ids, int br) : base(cookies)
        {
            this.Params.Add("ids", "[" + string.Join(",", ids) + "]");
            this.Params.Add("br", br);
        }

        public MusicUrl(int[] ids, int br) : this(new Hashtable(), ids, br)
        {

        }

        public MusicUrl(int[] ids) : this(ids, 999000)
        {

        }

        public MusicUrl(Hashtable cookies, int[] ids) : this(cookies, ids, 999000)
        {

        }



        public override string Url => "https://music.163.com/api/song/enhance/player/url";

        public override CryptoType Crypto => CryptoType.Netease_linuxApi;

        public override string Ua { get; }

        public override HttpMethod HttpMethod => HttpMethod.POST;

        public override string OptionUrl { get; }
    }
}
