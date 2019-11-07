using RequestHandler;
using System.Collections;

namespace NetEaseMusic_API.RequestOption.Options.Artist
{
    /// <summary>
    /// 获取歌手歌曲
    /// </summary>
    public sealed class ArtistMusics : BaseRequestOption
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cookies"></param>
        /// <param name="id">歌手Id</param>
        public ArtistMusics(Hashtable cookies, int id) : base(cookies)
        {
            Url = $"https://music.163.com/weapi/v1/artist/{id}";
        }

        public ArtistMusics(int id) : this(new Hashtable(), id)
        {
        }


        public override string Url { get; }

        public override CryptoType Crypto => CryptoType.Netease_weapi;

        public override string Ua { get; }

        public override HttpMethod HttpMethod => HttpMethod.POST;

        public override string OptionUrl { get; }
    }
}