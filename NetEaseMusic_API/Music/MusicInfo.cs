using System.Collections;
using System.Linq;

namespace MusicApi.NetEase.Music
{
    /// <summary>
    /// 音乐信息
    /// </summary>
    public sealed class MusicInfo : BaseRequestOption
    {
        public MusicInfo(Hashtable cookies, int id) : base(cookies)
        {
            int[] ids = { id };
            Params.Add("c", "[" + string.Join(",", ids.Select(id => "{\"id\":" + id + "}")) + "]");
            Params.Add("ids", "[" + string.Join(",", ids) + "]");
        }
        public MusicInfo(int id) : this(new Hashtable(), id)
        {

        }

        public override string Url => "https://music.163.com/weapi/v3/song/detail";

        public override CryptoType Crypto => CryptoType.Netease_weapi;

        public override string Ua { get; }

        public override HttpMethod HttpMethod => HttpMethod.POST;

        public override string OptionUrl { get; }
    }
}
