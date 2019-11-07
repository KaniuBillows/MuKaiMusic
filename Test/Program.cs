using System;
using System.Net.Http;

namespace Test
{

    class Program
    {
     
        static async System.Threading.Tasks.Task Main(string[] args)
        {

            HttpClient httpClient = new HttpClient();

            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            Convert.ToInt64(ts.TotalMilliseconds).ToString();
            string url = "http://www.kuwo.cn/url?format=mp3&rid=228908&response=url&type=convert_url3&br=128kmp3&from=web&t=1572748472375&reqId=c16902a6-d408-4fd0-7d65-fea8aa3ba5f2";
            var response = await httpClient.GetAsync(url);
            Console.WriteLine(await response.Content.ReadAsStringAsync());
        }




    }
    class Obj
    {
        public string fuck = "fuck";
        public string test = "test";
        public Func<string> func = new Func<string>(() => "");
        public string Test()
        {
            return "this is Test Method";
        }
    }

}
