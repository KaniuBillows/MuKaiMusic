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

            Search search = new Search("海阔天空");
            System.Console.WriteLine(await search.Request().Result.Content.ReadAsStringAsync());

        }




    }


}
