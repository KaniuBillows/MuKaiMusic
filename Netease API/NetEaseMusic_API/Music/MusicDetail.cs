using System.Collections;
using System.Linq;

namespace MusicApi.NetEase.Music
{
    /// <summary>
    /// 音乐信息
    /// </summary>
    public sealed class MusicDetail : BaseRequestOption
    {
        public MusicDetail(Hashtable cookies, long[] ids) : base(cookies)
        {

            Params.Add("c", "[" + string.Join(",", ids.Select(id => "{\"id\":" + id + "}")) + "]");
            Params.Add("ids", "[" + string.Join(",", ids) + "]");
        }
        public MusicDetail(int id) : this(new Hashtable(), new long[] { id })
        {

        }

        public MusicDetail(long[] ids) : this(new Hashtable(), ids)
        {

        }

        public override string Url => "https://music.163.com/weapi/v3/song/detail";

        public override CryptoType Crypto => CryptoType.Netease_weapi;

        public override string Ua { get; }

        public override HttpMethod HttpMethod => HttpMethod.POST;

        public override string OptionUrl { get; }
    }
}
