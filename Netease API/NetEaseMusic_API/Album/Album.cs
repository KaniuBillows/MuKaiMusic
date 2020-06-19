using System.Collections;

namespace MusicApi.NetEase.Album
{
    /// <summary>
    /// 获取专辑内容
    /// </summary>
    public sealed class Album : BaseRequestOption
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cookies">Cookies</param>
        /// <param name="id">专辑Id</param>
        public Album(Hashtable cookies, int id) : base(cookies)
        {
            Url = $"https://music.163.com/weapi/v1/album/{id}";
        }

        public Album(int id) : this(new Hashtable(), id)
        {
        }

        public override string Url { get; }

        public override CryptoType Crypto => CryptoType.Netease_weapi;

        public override string Ua { get; }

        public override HttpMethod HttpMethod => HttpMethod.POST;

        public override string OptionUrl { get; }
    }
}