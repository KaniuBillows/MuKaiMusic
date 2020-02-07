using System.Collections;

namespace MusicApi.NetEase.Similar
{
    /// <summary>
    /// 歌曲的相似歌曲
    /// </summary>
    public class SimilarMusic : BaseRequestOption
    {
        public SimilarMusic(Hashtable cookies, int musicId, int limit, int offset) : base(cookies)
        {
            this.Params.Add("songid", musicId);
            this.Params.Add("limit", limit);
            this.Params.Add("offset", offset);
        }

        public SimilarMusic(int musicId, int limit, int offset) : this(new Hashtable(), musicId, limit, offset)
        {

        }

        public SimilarMusic(int musicId) : this(musicId, 50, 0)
        {

        }

        public override string Url => "https://music.163.com/weapi/v1/discovery/simiSong";

        public override CryptoType Crypto => CryptoType.Netease_weapi;

        public override string Ua { get; }

        public override HttpMethod HttpMethod => HttpMethod.POST;

        public override string OptionUrl { get; }
    }
}
