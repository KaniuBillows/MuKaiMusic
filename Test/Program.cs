
using System.Threading;
using System.Threading.Tasks;

namespace Test
{

    class Program
    {

        static async Task Main(string[] args)
        {
            Mehtd();
            System.Console.WriteLine("run");
        }



        static Task Mehtd()
        {
            var t = new Task(() => Thread.Sleep(10000));
            t.Start();
            System.Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId}");
            return t;
        }


        static async Task MethodAsync()
        {
            System.Console.WriteLine($"in line 28 is {Thread.CurrentThread.ManagedThreadId}");
            await Task.Run(() =>
               {
                   System.Console.WriteLine($"Thread in line 31 {Thread.CurrentThread.ManagedThreadId}");//4
                   Thread.Sleep(2000);
               });
            System.Console.WriteLine($"Thread in line 34 is{Thread.CurrentThread.ManagedThreadId}");
            await Task.Run(() =>
            {

            });
            System.Console.WriteLine($"The Thread in line 39 {Thread.CurrentThread.ManagedThreadId}");
        }


    }


}
