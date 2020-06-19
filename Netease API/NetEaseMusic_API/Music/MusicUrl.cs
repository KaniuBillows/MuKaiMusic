using System.Collections;

namespace MusicApi.NetEase.Music
{
    /// <summary>
    /// 获取歌曲Url
    /// </summary>
    public class Music_Url : BaseRequestOption
    {
        public Music_Url(Hashtable cookies, long[] ids, int br) : base(cookies)
        {

            this.Params.Add("ids", "[" + string.Join(',', ids) + "]");
            this.Params.Add("br", br);
        }

        public Music_Url(long id, int br) : this(new Hashtable(), new long[] { id }, br)
        {

        }

        public Music_Url(long id) : this(id, 999000)
        {

        }

        public Music_Url(Hashtable cookies, long id) : this(cookies, new long[] { id }, 999000)
        {

        }



        public override string Url => "https://music.163.com/api/song/enhance/player/url";

        public override CryptoType Crypto => CryptoType.Netease_linuxApi;

        public override string Ua { get; }

        public override HttpMethod HttpMethod => HttpMethod.POST;

        public override string OptionUrl { get; }
    }
}
