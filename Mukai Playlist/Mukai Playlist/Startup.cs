using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Mukai_Playlist.Service;

namespace Mukai_Playlist
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            //mongoDB数据库
            services.AddSingleton<IMongoDatabase>((provider) =>
            {
                var client = new MongoClient(Configuration.GetConnectionString("MongoDB"));
                return client.GetDatabase("mukai");
            });

            services.AddAuthentication()
            .AddJwtBearer(Configuration["Secret"], option =>
                {
                    option.Authority = "kaniu.pro";
                    option.Audience = "kaniu.pro";
                    option.RequireHttpsMetadata = false;
                });
            services.AddScoped<PlaylistService>();
            services.AddSingleton<IHostedService, ConsulHostedService>();
             services.AddSingleton<IConsulClient, ConsulClient>(provider =>
            new ConsulClient(consulConfig =>
            {
                consulConfig.Address = new Uri(Configuration["ConsulAddress"]);
            }));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
