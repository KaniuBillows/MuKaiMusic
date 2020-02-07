using System.Collections;

namespace MusicApi.NetEase.Artist
{
    /// <summary>
    /// 获取歌手描述
    /// </summary>
    public sealed class ArtistDescription : BaseRequestOption
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cookies"></param>
        /// <param name="id">歌手Id</param>
        public ArtistDescription(Hashtable cookies, int id) : base(cookies)
        {
            Params.Add("id", id);
        }
        public ArtistDescription(int id) : this(new Hashtable(), id)
        {

        }

        public override string Url => "https://music.163.com/weapi/artist/introduction";

        public override CryptoType Crypto => CryptoType.Netease_weapi;

        public override string Ua { get; }

        public override HttpMethod HttpMethod => HttpMethod.POST;

        public override string OptionUrl { get; }
    }
}
