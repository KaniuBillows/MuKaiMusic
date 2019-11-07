using RequestHandler;
using System;
using System.Collections;

namespace MiGu_Music_API
{
    public class Mobile_Listen_Url : BaseRequestOption
    {
        public Mobile_Listen_Url(Hashtable cookies) : base(cookies)
        {
        }
        public Mobile_Listen_Url(string contentId) : this(new Hashtable())
        {
            this.Params.Add("contentId", contentId);
            this.Params.Add("toneFlag", "PQ");
        }

        public override string Url { get; } = "https://app.c.nf.migu.cn/MIGUM2.0/v2.0/content/listen-url?";

        public override CryptoType Crypto { get; } = CryptoType.MiGU_Mobile;

        public override string Ua { get; } = "Andorid_migu";

        public override HttpMethod HttpMethod { get; } = HttpMethod.GET;

        public override string OptionUrl { get; }

        public override string GetQueryString()
        {
            string query = string.Empty;
            foreach (var key in this.Params.Keys)
            {
                query += key + "=" + Uri.EscapeDataString(Params[key].ToString()) + "&";
            }
            return query.Substring(0, query.LastIndexOf("&"));
        }
    }
}
