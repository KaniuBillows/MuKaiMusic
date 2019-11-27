using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MuKai_Music.DataContext;
using MuKai_Music.Filter;
using MuKai_Music.middleware;
using MuKai_Music.Model.Service;
using MuKai_Music.Service;
using System;

namespace MuKai_Music
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
            services.AddSwaggerDocument();
            services.AddControllersWithViews();
            services.AddSingleton<Func<HttpContext, MusicService>>((HttpContext httpContext) =>
            {
                var optionsBuilder = new DbContextOptionsBuilder<MusicContext>();
                var conf = Configuration.GetConnectionString("MySql");
                optionsBuilder.UseMySql(conf);
                return new MusicService(httpContext, new MusicContext(optionsBuilder.Options));
            });
            services.AddSingleton<Func<HttpContext, UserService>>((HttpContext httpContext) => new UserService(httpContext));
            services.AddScoped<MyAuthorFilter>();
            services.AddScoped<MyActionFilter>();
            services.AddDbContext<MusicContext>(options =>
            {
                //var optionsBuilder = new DbContextOptionsBuilder<MiguContext>();
                options.UseMySql(Configuration.GetConnectionString("MySql"));
            });
            /*services.AddSingleton<Func<MiguContext>>(() =>
            {
                var optionsBuilder = new DbContextOptionsBuilder<MiguContext>();
                optionsBuilder.UseMySql(Configuration.GetConnectionString("MySql"));
                return new MiguContext(optionsBuilder.Options);
            });*/

            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "mukaiMusic/dist/mukaiMusic";
            });
            services.AddMvc(option =>
            {
                /*客户端缓存*/
                option.CacheProfiles.Add("default", new Microsoft.AspNetCore.Mvc.CacheProfile
                {
                    Duration = 86400 /*24小时资源缓存*/
                });
            });
            //响应式缓存
            services.AddResponseCaching();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            //  app.UseOptions();
            //配置https重定向
            // app.UseHttpsRedirection();
            //配置启用静态资源文件
            app.UseStaticFiles();
            app.UseOpenApi();
            //使用swagger
            app.UseSwaggerUi3();
            //启用编译完成的单页面静态资源文件
            if (!env.IsDevelopment())
            {
                app.UseSpaStaticFiles();
            }
            app.UseRouting();
            //允许跨域
            app.UseCors(builder =>
            {
                builder.AllowAnyOrigin();
                builder.AllowAnyMethod();
                builder.AllowAnyHeader();
            });
            app.UseResponseCaching();

            app.Use(async (context, next) =>
            {
                context.Response.GetTypedHeaders().CacheControl =
                    new Microsoft.Net.Http.Headers.CacheControlHeaderValue()
                    {
                        Public = true,
                        MaxAge = TimeSpan.FromSeconds(86400)
                    };
                context.Response.Headers[Microsoft.Net.Http.Headers.HeaderNames.Vary] =
                    new string[] { "Accept-Encoding" };

                await next();
            });
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpaStaticFiles();
            /*    app.UseSpa(spa =>
                {
                      // To learn more about options for serving an Angular SPA from ASP.NET Core,
                      // see https://go.microsoft.com/fwlink/?linkid=864501

                      spa.Options.SourcePath = "mukaiMusic";

                    if (env.IsDevelopment())
                    {
                        spa.UseAngularCliServer(npmScript: "start");
                    }
                });*/
        }
    }
}
