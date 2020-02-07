using MusicApi.Migu.Search;

namespace Test
{

    class Program
    {

        static async System.Threading.Tasks.Task Main(string[] args)
        {
            Mobile_Search search = new Mobile_Search(1, 30, "周杰伦");
            var message = await search.Request();
            System.Console.WriteLine(await message.Content.ReadAsStringAsync());
        }




    }


}
