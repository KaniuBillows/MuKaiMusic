using System.Collections;

namespace MusicApi.NetEase.Banner
{
    public enum BannerType
    {
        pc,
        android,
        iphone,
        ipad
    }
    /// <summary>
    /// 首页轮播图
    /// </summary>
    public class Banner : BaseRequestOption
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type">"pc","android","iphone","ipad"</param>
        public Banner(Hashtable cookies, BannerType type) : base(cookies)
        {
            this.Params.Add("clientType", type.ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type">"pc","android","iphone","ipad"</param>
        public Banner(BannerType type) : this(new Hashtable(), type) { }

        public Banner() : this(BannerType.pc) { }

        public override string Url => "https://music.163.com/api/v2/banner/get";

        public override CryptoType Crypto => CryptoType.Netease_linuxApi;

        public override string Ua { get; }

        public override HttpMethod HttpMethod => HttpMethod.POST;

        public override string OptionUrl { get; }
    }
}
