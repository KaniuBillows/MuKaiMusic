using System.Collections;

namespace MusicApi.NetEase.User
{
    public class LoginPhone : BaseRequestOption
    {
        public LoginPhone(Hashtable cookies, string countryCode, string phone, string password) : base(cookies)
        {
            this.Cookies.Add("os", "pc");
            this.Params.Add("phone", phone);
            this.Params.Add("countrycode", 86);
            this.Params.Add("password", password);//客户端进行加密
            this.Params.Add("rememberLogin", "true");
        }

        public override string Url => "https://music.163.com/weapi/login/cellphone";

        public override CryptoType Crypto => CryptoType.Netease_weapi;

        public override string Ua { get; }

        public override HttpMethod HttpMethod => HttpMethod.POST;

        public override string OptionUrl { get; }
    }
}
