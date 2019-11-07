using RequestHandler;
using System;
using System.Collections;

namespace Kuwo_Music_Api.Url
{
    public class Get_Url : BaseRequestOption
    {
        public Get_Url(Hashtable cookies, int rid, int br) : base(cookies)
        {
            var id = Guid.NewGuid();
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            var t = Convert.ToInt64(ts.TotalMilliseconds);
            this.Url = $"http://www.kuwo.cn/url?format=mp3&rid={rid.ToString()}&response=url&type=convert_url3&br={br.ToString()}kmp3&from=web&t={t.ToString()}&reqId={id.ToString()}";
        }

        public Get_Url(int rid, int br) : this(new Hashtable(), rid, br)
        {

        }

        public override string Url { get; }

        public override CryptoType Crypto => CryptoType.KuWo_Web;

        public override string Ua => "pc";

        public override HttpMethod HttpMethod => HttpMethod.POST;

        public override string OptionUrl { get; }
    }
}
