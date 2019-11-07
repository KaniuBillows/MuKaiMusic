using RequestHandler;
using System.Collections;

namespace NetEaseMusic_API.RequestOption.Options.Music
{
    /// <summary>
    /// 推荐新歌
    /// </summary>
    public sealed class NewMusicPersonalized : BaseRequestOption
    {
        public NewMusicPersonalized(Hashtable cookies) : base(cookies)
        {
            Params.Add("type", "recommend");
        }
        public NewMusicPersonalized() : this(new Hashtable())
        {

        }

        public override string Url { get; } = "https://music.163.com/weapi/personalized/newsong";

        public override CryptoType Crypto => CryptoType.Netease_weapi;

        public override string Ua { get; }

        public override HttpMethod HttpMethod => HttpMethod.POST;

        public override string OptionUrl { get; }
    }
}