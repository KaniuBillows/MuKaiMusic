using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MuKai_Music.Extions.Util
{
    public static class HttpClientExtension
    {
        public static async Task<HttpResponseMessage> PostAsync<T>(this HttpClient client, string url, T body)
        {
            string content = JsonSerializer.Serialize<T>(body);
            HttpContent param = new StringContent(content, Encoding.UTF8, "application/json");

            return await client.PostAsync(url, param);
        }
    }
}
