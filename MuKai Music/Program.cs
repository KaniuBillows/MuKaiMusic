using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;

namespace MuKai_Music
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                  .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile("appsettings.json")
                .Build();

            return WebHost.CreateDefaultBuilder(args).UseConfiguration(configuration)
                 .UseStartup<Startup>()
                 .ConfigureAppConfiguration((hostingContext, config) =>
                     config.AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json")
                 );
        }

    }
}
