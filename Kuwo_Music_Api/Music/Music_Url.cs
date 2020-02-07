using System;
using System.Collections;

namespace MusicApi.Kuwo.Music
{
    public class Music_Url : BaseRequestOption
    {
        private Music_Url(Hashtable cookies, int rid) : base(cookies)
        {
            var id = Guid.NewGuid();
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            long t = Convert.ToInt64(ts.TotalMilliseconds);
            this.Url = $"http://www.kuwo.cn/url?format=mp3&rid={rid.ToString()}&response=url&type=convert_url3&from=web&t={t.ToString()}&reqId={id.ToString()}";
        }

        public Music_Url(int rid) : this(new Hashtable(), rid)
        {

        }

        public override string Url { get; }

        public override CryptoType Crypto => CryptoType.KuWo_Web;

        public override string Ua => "pc";

        public override HttpMethod HttpMethod => HttpMethod.POST;

        public override string OptionUrl { get; }
    }
}
