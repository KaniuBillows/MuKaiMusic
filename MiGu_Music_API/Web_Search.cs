using RequestHandler;
using System;
using System.Collections;

namespace MiGu_Music_API
{
    public class Web_Search : BaseRequestOption
    {
        public Web_Search(Hashtable cookies, string keyWord) : base(cookies)
        {
            this.Url = $"http://music.migu.cn/v2/async/search?keyword={Uri.EscapeDataString(keyWord)}";
        }

        public Web_Search(string keyWord) : this(new Hashtable(), keyWord)
        {

        }

        public override string Url { get; }

        public override CryptoType Crypto => CryptoType.MiGU_Web;

        public override string Ua { get; }

        public override HttpMethod HttpMethod => HttpMethod.GET;

        public override string OptionUrl { get; }
    }
}
