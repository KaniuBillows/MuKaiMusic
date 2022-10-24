using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace Kuwo_API
{
    public class MusicService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMemoryCache _memoryCache;

        public MusicService(IHttpClientFactory httpClientFactory, IMemoryCache memoryCache)
        {
            _httpClientFactory = httpClientFactory;
            _memoryCache = memoryCache;
        }

        public async Task<string> GetKuwoToken()
        {
            using var client = this._httpClientFactory.CreateClient();
            var response = await client.GetAsync("http://www.kuwo.cn");
          
            if (!response.Headers.TryGetValues("Set-Cookie", out var values)) return null;
            using var enumerator = values.GetEnumerator();
            if (!enumerator.MoveNext()) return null;
            var value = enumerator.Current;
            value = value.Substring(9, 10);
            _memoryCache.Set("token", value);
            return value;
        }
    }
}