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
using MuKai_Music.Middleware.ApiCache;
using MuKai_Music.Middleware.TokenManager;
using MuKai_Music.Model.Service;
using MuKai_Music.Service;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace MuKai_Music
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            config = configuration;
            TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = true,//�Ƿ���֤Issuer
                ValidateAudience = true,//�Ƿ���֤Audience
                ValidateLifetime = true,//�Ƿ���֤ʧЧʱ��
                ClockSkew = TimeSpan.FromSeconds(30),
                ValidateIssuerSigningKey = true,//�Ƿ���֤SecurityKey
                ValidAudience = configuration.GetDomain(),//Audience
                ValidIssuer = configuration.GetDomain(),//Issuer���������ǰ��ǩ��jwt������һ��
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetSecurityKey()))
            };
            JsonSerializerOptions = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true
            };
        }

        private readonly IConfiguration config;


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

            //todo ���ݿ�
            services.AddSingleton((provider) =>
            {
                var client = new MongoClient(config.GetConnectionString("MongoDB"));
                return client.GetDatabase("mukai");
            });

            services.AddSingleton<MusicService>();

            services.AddSingleton<PlaylistService>();

            services.AddSingleton<TokenProvider>();

            services.AddHttpClient();

            services.AddGrpcClient<AccountService.AccountServiceClient>(o =>
            o.Address = new Uri(config.GetAccountAddress()))
                .ConfigurePrimaryHttpMessageHandler(() =>
                {
                    var handler = new HttpClientHandler
                    {
                        ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                    };
                    return handler;
                });

            //���Ĭ��tokenHandler
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            //����json web token ����֤����
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
            
            //��ӻ������
            services.AddICache(option =>
            {
                option.Age = int.Parse(config.GetCacheAge());
                option.CacheType = Enum.Parse<CacheType>(config.GetCacheType());
            });

            services.AddMvc(option =>
            {
                /*�ͻ��˻���*/
                option.CacheProfiles.Add("default", new Microsoft.AspNetCore.Mvc.CacheProfile
                {
                    Duration = 86400 /*24Сʱ��Դ����*/
                });
                option.CacheProfiles.Add("longTime", new Microsoft.AspNetCore.Mvc.CacheProfile
                {
                    Duration = 86400 * 7 /*7�컺��*/
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

            app.UseTokenManager();

            //�������þ�̬��Դ�ļ�
            app.UseStaticFiles();
            app.UseOpenApi();
            //ʹ��swagger
            app.UseSwaggerUi3();

            app.UseRouting();

            //�������
            app.UseCors(builder =>
            {
                builder.AllowAnyOrigin();
                builder.AllowAnyMethod();
                builder.AllowAnyHeader();
            });

            //�����֤
            app.UseAuthentication();
            //��Ȩ
            app.UseAuthorization();

            app.UseMiddleware<DecryptMiddleware>();


            app.UseApiCacheMiddleware();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });
            //���õ�ҳ�澲̬��Դ�ļ�
            if (env.IsProduction())
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
