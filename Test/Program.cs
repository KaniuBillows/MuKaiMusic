using System.Net.Http;
using System.Threading.Tasks;

namespace Test
{

    class Program
    {

        static async Task Main(string[] args)
        {
            using HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Host", "music.migu.cn");
            client.DefaultRequestHeaders.Add("Referer", "http://music.migu.cn/v3/music/player/audio");
            var res = await client.GetAsync("http://music.migu.cn/v3/api/music/audioPlayer/getLyric?copyrightId=63273403099");
            System.Console.WriteLine(await res.Content.ReadAsStringAsync());
        }






    }


}
