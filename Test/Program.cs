using MusicApi.Migu.Music;
using MusicApi.Migu.Search;
using MusicApi.NetEase.Search;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace Test
{

    class Program
    {

        static async System.Threading.Tasks.Task Main(string[] args)
        {

            HttpClient client = new HttpClient();
            //client.DefaultRequestHeaders.Add("Referer", "http://music.migu.cn/v3/music/player/audio");
            string id = "3215";
            var result = await client.GetAsync($"http://music.migu.cn/v3/api/music/audioPlayer/getSongPic?songId={id}");
            System.Console.WriteLine(await result.Content.ReadAsStringAsync());
        }




    }


}
