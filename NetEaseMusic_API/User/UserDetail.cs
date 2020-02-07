using System.Collections;

namespace MusicApi.NetEase.User
{
    /// <summary>
    /// 用户详情
    /// </summary>
    public sealed class UserDetail : BaseRequestOption
    {
        public UserDetail(Hashtable cookies, int id) : base(cookies)
        {
            this.Url = $"https://music.163.com/weapi/v1/user/detail/{id}";
        }

        public UserDetail(int id) : this(new Hashtable(), id)
        {
        }

        public override string Url { get; }

        public override CryptoType Crypto => CryptoType.Netease_weapi;

        public override string Ua { get; }

        public override HttpMethod HttpMethod => HttpMethod.POST;

        public override string OptionUrl { get; }
    }
}
