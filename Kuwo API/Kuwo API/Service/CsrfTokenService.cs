using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;

namespace Kuwo_API
{
    public class CsrfTokenService : IHostedService
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IMemoryCache _memoryCache;
        private CancellationTokenSource _cts;

        public CsrfTokenService(IHttpClientFactory clientFactory, IMemoryCache memoryCache)
        {
            _clientFactory = clientFactory;
            _memoryCache = memoryCache;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            using HttpClient client = this._clientFactory.CreateClient();
            HttpResponseMessage response = await client.GetAsync("http://www.kuwo.cn", cancellationToken);
            if (!response.Headers.TryGetValues("Set-Cookie", out var values)) return;
            using var enumerator = values.GetEnumerator();
            if (!enumerator.MoveNext()) return;
            var value = enumerator.Current;
            value = value.Substring(9, 10);
            _memoryCache.Set("token", value);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _cts.Cancel();
            return Task.CompletedTask;
        }
    }
}