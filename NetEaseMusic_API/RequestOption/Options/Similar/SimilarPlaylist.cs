using RequestHandler;
using System.Collections;

namespace NetEaseMusic_API.RequestOption.Options.Similar
{
    /// <summary>
    /// 获取相似歌单
    /// </summary>
    public sealed class SimilarPlaylist : BaseRequestOption
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cookies"></param>
        /// <param name="musicId">音乐Id</param>
        /// <param name="limit"></param>
        /// <param name="offset"></param>
        public SimilarPlaylist(Hashtable cookies, int musicId, int limit, int offset) : base(cookies)
        {
            Params.Add("songid", musicId);
            Params.Add("limit", limit);
            Params.Add("offset", offset);
        }
        public SimilarPlaylist(int musicId, int limit, int offset) : this(new Hashtable(), musicId, limit, offset)
        {
        }


        public override string Url => "https://music.163.com/weapi/discovery/simiPlaylist";

        public override CryptoType Crypto => CryptoType.Netease_weapi;

        public override string Ua { get; }

        public override HttpMethod HttpMethod => HttpMethod.POST;

        public override string OptionUrl { get; }
    }
}
