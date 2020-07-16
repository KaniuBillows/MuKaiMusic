using System;
using Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Mukai_Account.Filters;
using Mukai_Auth.DataContext;
using Mukai_Auth.Service;
using MuKai_Auth;

namespace Mukai_Auth
{
    public class Startup
    {

        public static IConfiguration Configuration { get; set; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddDbContext<AccountContext>(option =>
            {
                option.UseNpgsql(Configuration.GetConnectionString("PostgreSql"));
            });
            services.AddAuthentication()
                 .AddJwtBearer(Configuration["Secret"], option =>
                 {
                     option.Authority = "kaniu.pro";
                     option.Audience = "kaniu.pro";
                     option.RequireHttpsMetadata = false;
                 });
            services.AddAuthorization();
            services.AddSingleton<IHostedService, ConsulHostService>();
            services.AddSingleton<IConsulClient, ConsulClient>(provider =>
            new ConsulClient(consulConfig =>
            {
                consulConfig.Address = new Uri(Configuration["ConsulAddress"]);
            }));
            services.AddSingleton<TokenProvider>();
            services.AddSingleton<EncryptAttribute>(serviceProvider =>
            {
                return new EncryptAttribute(Configuration);
            });
            services.AddHttpClient();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAuthentication();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });
        }
    }
}
