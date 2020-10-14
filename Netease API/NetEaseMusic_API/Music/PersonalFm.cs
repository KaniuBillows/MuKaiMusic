using System.Collections;

namespace MusicApi.NetEase.Music
{
    public class PersonalFm : BaseRequestOption
    {
        public PersonalFm(Hashtable cookies) : base(cookies)
        {
        }

        public override string Url => "https://music.163.com/weapi/v1/radio/get";
        public override string OptionUrl { get; }
        public override CryptoType Crypto => CryptoType.Netease_weapi;
        public override string Ua { get; }
        public override HttpMethod HttpMethod => HttpMethod.POST;
    }
}