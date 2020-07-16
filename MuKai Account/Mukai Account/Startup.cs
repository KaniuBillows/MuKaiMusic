using System;
using Aliyun.OSS;
using Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Mukai_Account.Data;
using Mukai_Account.Filters;
using Mukai_Account.Service;
using Mukai_Account.Service.Interface;
namespace Mukai_Account
{
    public class Startup
    {

        public IConfiguration Configuration { get; set; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddScoped<Chloe.IDbContext>((serviceProvider) =>
             new Chloe.PostgreSQL.PostgreSQLContext(
             new PostgreSQLConnectionFactory(Configuration.GetConnectionString("PostgreSQL"))));

            services.AddAuthentication()
           .AddJwtBearer(Configuration["Secret"], option =>
           {
               option.Authority = "kaniu.pro";
               option.Audience = "kaniu.pro";
               option.RequireHttpsMetadata = false;
           });
            services.AddAuthorization();

            //阿里云对象存储
            services.AddScoped<IOss, OssClient>(servicesProvider =>
            {
                return new OssClient(Configuration["OssEndPoint"], Configuration["AccessKeyId"], Configuration["AccessKeySecret"]);
            });

            //Consul服务发现
            services.AddSingleton<IConsulClient, ConsulClient>(p => new ConsulClient(consulConfig =>
            {
                consulConfig.Address = new Uri(Configuration.GetValue<string>("ConsulAddress"));
            }));
            services.AddSingleton<IHostedService, ConsulHostService>();

            services.AddScoped<AccountService>();
            services.AddScoped<IFileService, OssFileService>();
            services.AddSingleton<EncryptAttribute>(serviceProvider =>
           {
               return new EncryptAttribute(Configuration);
           });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();


            app.UseEndpoints(endpoints =>
           {
               endpoints.MapControllerRoute(
                   name: "default",
                   pattern: "{controller}/{action=Index}/{id?}");
           });
        }
    }
}
