using System;
using System.Net.Http;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using MuKai_Music.Test;

namespace Test
{
    public abstract class BaseTest
    {
        public HttpClient Client { get; }
        public IServiceProvider ServiceProvider { get; set; }
        public BaseTest()
        {
         /*   IConfigurationRoot configuration = new ConfigurationBuilder()
             .SetBasePath("D:/Programs/KaniuBillows/MuKai Music")
             .AddJsonFile("appsettings.json")
             .Build();*/
            var server = new TestServer(WebHost.CreateDefaultBuilder().UseStartup<TestStartup>()
             //   .UseConfiguration(configuration)
                 .ConfigureAppConfiguration((hostingContext, config) =>
                     config.AddJsonFile($"appsettings.Development.json")
                 ));
            Client = server.CreateClient();
            ServiceProvider = server.Services;
        }
    }
}
