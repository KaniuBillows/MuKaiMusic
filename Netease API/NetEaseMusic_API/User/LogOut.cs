using System.Collections;

namespace MusicApi.NetEase.User
{
    /// <summary>
    /// 注销登录
    /// </summary>
    public class LogOut : BaseRequestOption
    {
        public LogOut(Hashtable cookies) : base(cookies)
        {
        }

        public override string Url => "https://music.163.com/weapi/logout";

        public override CryptoType Crypto => CryptoType.Netease_weapi;

        public override string Ua => "pc";

        public override HttpMethod HttpMethod => HttpMethod.POST;

        public override string OptionUrl { get; }
    }
}
