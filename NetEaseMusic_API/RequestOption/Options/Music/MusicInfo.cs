using RequestHandler;
using System.Collections;
using System.Linq;

namespace NetEaseMusic_API.RequestOption.Options.Music
{
    /// <summary>
    /// 音乐信息
    /// </summary>
    public sealed class MusicInfo : BaseRequestOption
    {
        public MusicInfo(Hashtable cookies, int[] ids) : base(cookies)
        {

            Params.Add("c", "[" + string.Join(",", ids.Select(id => "{\"id\":" + id + "}")) + "]");
            Params.Add("ids", "[" + string.Join(",", ids) + "]");
        }
        public MusicInfo(int[] ids) : this(new Hashtable(), ids)
        {

        }

        public override string Url => "https://music.163.com/weapi/v3/song/detail";

        public override CryptoType Crypto => CryptoType.Netease_weapi;

        public override string Ua { get; }

        public override HttpMethod HttpMethod => HttpMethod.POST;

        public override string OptionUrl { get; }
    }
}
