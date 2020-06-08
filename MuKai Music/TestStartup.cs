using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using MuKai_Account;
using MuKai_Account.Middleware;
using MuKai_Music.Cache;
using MuKai_Music.Middleware;
using MuKai_Music.Model.Service;
using MuKai_Music.Service;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace MuKai_Music.Test
{
    public class TestStartup
    {
        public TestStartup(IConfiguration configuration)
        {
            Configuration = configuration;
            TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = true,//是否验证Issuer
                ValidateAudience = true,//是否验证Audience
                ValidateLifetime = true,//是否验证失效时间
                ClockSkew = TimeSpan.FromSeconds(30),
                ValidateIssuerSigningKey = true,//是否验证SecurityKey
                ValidAudience = Configuration.GetDomain(),//Audience
                ValidIssuer = Configuration.GetDomain(),//Issuer，这两项和前面签发jwt的设置一致
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration.GetSecurityKey()))
            };
            JsonSerializerOptions = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true
            };
        }

        public static IConfiguration Configuration { get; private set; }


        public static JsonSerializerOptions JsonSerializerOptions { get; private set; }

        public static TokenValidationParameters TokenValidationParameters { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
#if DEBUG
            AppContext.SetSwitch(
   "System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
#endif
            services.AddSwaggerDocument();
            services.AddControllersWithViews();

            //todo 数据库
            services.AddSingleton((provider) =>
            {
                var client = new MongoClient(Configuration.GetConnectionString("MongoDB"));
                return client.GetDatabase("mukai_test");
            });

            services.AddSingleton<IConfiguration>((provider) => TestStartup.Configuration);

            services.AddSingleton<MusicService>();

            services.AddSingleton<PlaylistService>();

            services.AddSingleton<RedisClient>();

            services.AddSingleton<TokenProvider>();

            services.AddHttpClient();

            services.AddGrpcClient<AccountService.AccountServiceClient>(o =>
            o.Address = new Uri(Configuration.GetAccountAddress()))
                .ConfigurePrimaryHttpMessageHandler(() =>
                {
                    var handler = new HttpClientHandler
                    {
                        ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                    };
                    return handler;
                });

            //清除默认tokenHandler
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            //启用json web token 做验证方案
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }
            ).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = TokenValidationParameters;
            });

            services.AddHttpContextAccessor();

            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration => configuration.RootPath = "mukaiMusic/dist/muKaiMusic");

            //添加缓存
            services.AddICache(option =>
            {
                option.Age = int.Parse(Configuration.GetCacheAge());
                option.CacheType = Enum.Parse<CacheType>(Configuration.GetCacheType());
            });

            services.AddMvc(option =>
            {
                /*客户端缓存*/
                option.CacheProfiles.Add("default", new Microsoft.AspNetCore.Mvc.CacheProfile
                {
                    Duration = 86400 /*24小时资源缓存*/
                });
                option.CacheProfiles.Add("longTime", new Microsoft.AspNetCore.Mvc.CacheProfile
                {
                    Duration = 86400 * 7 /*7天缓存*/
                });

            }).AddJsonOptions(configure =>
                    configure.JsonSerializerOptions.PropertyNameCaseInsensitive = true);

            services.AddResponseCaching();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                //  app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            //app.UseTokenManager();

            //配置启用静态资源文件
            app.UseStaticFiles();
            app.UseOpenApi();
            //使用swagger
            app.UseSwaggerUi3();

            app.UseRouting();

            //允许跨域
            app.UseCors(builder =>
            {
                builder.AllowAnyOrigin();
                builder.AllowAnyMethod();
                builder.AllowAnyHeader();
            });

            //身份认证
            app.UseAuthentication();
            //授权
            app.UseAuthorization();

            app.UseMiddleware<DecryptMiddleware>();


            //app.UseApiCacheMiddleware();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });
            //启用单页面静态资源文件
            if (!env.IsProduction())
            {
                app.UseSpaStaticFiles();
                app.UseSpa(spa =>
                {
                    spa.Options.SourcePath = "mukaiMusic";

                    /*if (env.IsDevelopment())
                    {
                        spa.UseAngularCliServer(npmScript: "start");
                    }*/
                });
            }
        }
    }
}
