using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace Test
{
    public static class TestUtil
    {
        public static async Task<R> PostAsync<T, R>(this HttpClient httpClient, string uri, T content)
        {
            var body = new StringContent(JsonSerializer.Serialize(content), Encoding.UTF8, "application/json");
            var response = httpClient.PostAsync(uri, body);
            var s = await response.Result.Content.ReadAsStringAsync();
            Assert.NotEmpty(s);
            var result = JsonSerializer.Deserialize<R>(s);
            return result;
        }

        public static async Task<R> GetAsync<R>(this HttpClient httpClient, string uri)
        {
            var response = httpClient.GetAsync(uri);
            var s = await response.Result.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<R>(s);
            return result;
        }

        public static async Task<R> PutAsync<T, R>(this HttpClient httpClient, string uri, T content)
        {
            var body = new StringContent(JsonSerializer.Serialize(content), Encoding.UTF8, "application/json");
            var response = httpClient.PutAsync(uri, body);
            var s = await response.Result.Content.ReadAsStringAsync();
            Assert.NotEmpty(s);
            var result = JsonSerializer.Deserialize<R>(s);
            return result;
        }

        public static async Task<R> DeleteAsync<R>(this HttpClient client, string uri)
        {
            var response = client.DeleteAsync(uri);
            var s = await response.Result.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<R>(s);
            return result;
        }
    }
}
