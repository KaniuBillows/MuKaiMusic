using System.Collections;
using System.Net.Http;
using System.Threading.Tasks;

namespace RequestHandler
{
    public enum HttpMethod
    {
        GET,
        POST,
        PUT,
        DELETE
    }

    public enum CryptoType
    {
        MiGU_Web,
        MiGU_Mobile,
        Netease_linuxApi,
        Netease_weapi,
        Netease_eapi,
        KuWo_Web
    }

    public abstract class BaseRequestOption : IRequestOption
    {
        protected BaseRequestOption(Hashtable cookies)
        {
            this.Cookies = cookies;
        }
        public async virtual Task<HttpResponseMessage> Request()
        {
            return await RequestSender.Send(this);
        }
        public virtual Hashtable Params { get; } = new Hashtable();
        public Hashtable Cookies { get; }
        public abstract string Url { get; }
        public abstract CryptoType Crypto { get; }
        public abstract string Ua { get; }
        public abstract HttpMethod HttpMethod { get; }
        public abstract string OptionUrl { get; }
        public virtual string GetQueryString()
        {
            return "";
        }

    }
}
