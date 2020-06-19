using System.Collections;

namespace MusicApi.NetEase.Similar
{
    /// <summary>
    /// 获取相似歌手
    /// </summary>
    public sealed class SimilarArtist : BaseRequestOption
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cookies"></param>
        /// <param name="id">歌手Id</param>
        public SimilarArtist(Hashtable cookies, int id) : base(cookies)
        {
            this.Params.Add("artistid", id);
        }

        public SimilarArtist(int id) : this(new Hashtable(), id)
        {

        }

        public override string Url => "https://music.163.com/weapi/discovery/simiArtist";

        public override CryptoType Crypto => CryptoType.Netease_weapi;

        public override string Ua { get; }

        public override HttpMethod HttpMethod => HttpMethod.POST;

        public override string OptionUrl { get; }
    }
}
