using System;
using System.Collections;

namespace MusicApi.Kuwo.Search
{
    public class Music_Search : BaseRequestOption
    {
        public Music_Search(string token, string keyword, int limit, int offset) : base(new Hashtable() {
            { "kw_token",token }
        })
        {
            var id = Guid.NewGuid();
            this.Url = $"http://www.kuwo.cn/api/www/search/searchMusicBykeyWord?key={Uri.EscapeDataString(keyword)}&pn={offset + 1}&rn={limit}&reqId={id.ToString()}";
        }



        public Music_Search(string token, string keyword) : this(token, keyword, 10, 0)
        {

        }

        public override string Url { get; }

        public override CryptoType Crypto => CryptoType.KuWo_Web;

        public override string Ua => "pc";

        public override HttpMethod HttpMethod => HttpMethod.GET;

        public override string OptionUrl { get; }
    }
}
