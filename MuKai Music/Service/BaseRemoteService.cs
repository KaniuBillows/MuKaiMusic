using DataAbstract;
using Microsoft.Extensions.Configuration;
using MuKai_Music.Extions.Util;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MuKai_Music.Service
{
    public abstract class BaseRemoteService
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IConfiguration configuration;

        public BaseRemoteService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            this.httpClientFactory = httpClientFactory;
            this.configuration = configuration;
        }

        protected async Task<string> ServiceRequest(DataSource dataSource, string route)
        {
            StringBuilder builder = new StringBuilder(configuration.GetBaseUrl(dataSource));
            builder.Append(route);
            return await (await this.RemoteGetAsync(builder.ToString())).Content.ReadAsStringAsync();
        }

        protected async Task<T> ServiceRequest<T>(DataSource dataSource, string route)
        {
            return JsonSerializer.Deserialize<T>(await this.ServiceRequest(dataSource, route));
        }

        protected async Task<HttpResponseMessage> RemoteGetAsync(string url)
        {
            using HttpClient client = httpClientFactory.CreateClient();
            return await client.GetAsync(url);
        }

        public async Task<HttpResponseMessage> RemotePostAsync<T>(string url, T content)
        {
            using HttpClient client = httpClientFactory.CreateClient();
            return await client.PostAsync(url, content);
        }
    }
}
