using RequestHandler;
using System;
using System.Collections;

namespace Kuwo_Music_Api.Search
{
    public class Music_Search : BaseRequestOption
    {
        public Music_Search(Hashtable cookies, string keyword) : base(cookies)
        {
            var id = Guid.NewGuid();
            this.Url = $"http://www.kuwo.cn/api/www/search/searchMusicBykeyWord?key={Uri.EscapeDataString(keyword)}pn=1&rn=30&reqId={id.ToString()}";
        }

        public override string Url { get; }

        public override CryptoType Crypto => CryptoType.KuWo_Web;

        public override string Ua => "pc";

        public override HttpMethod HttpMethod => HttpMethod.GET;

        public override string OptionUrl { get; }
    }
}
