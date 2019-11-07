using System.Collections;
using System.Net.Http;
using System.Threading.Tasks;

namespace RequestHandler
{
    public interface IRequestOption
    {
        Hashtable Params { get; }
        string Url { get; }
        string OptionUrl { get; }
        CryptoType Crypto { get; }
        string Ua { get; }
        Hashtable Cookies { get; }
        HttpMethod HttpMethod { get; }
        string GetQueryString();
        Task<HttpResponseMessage> Request();
    }
}
