using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using MuKai_Music.DataContext;
using MuKai_Music.Middleware;
using System;
using MuKai_Music.Model.DataEntity;
using Microsoft.AspNetCore.Identity;
using MuKai_Music.Model.Manager;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using MuKai_Music.Extensions.Store;
using Microsoft.AspNetCore.SpaServices.AngularCli;

namespace MuKai_Music
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            Config = configuration;
        }

        public IConfiguration Configuration { get; }

        public static IConfiguration Config { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSwaggerDocument();
            services.AddControllersWithViews();
            /*
             * TODO ��������Ϣ�־û����������ݿ�
             */

            //���ݿ�����������
            services.AddDbContext<MusicContext>(options =>
                options.UseNpgsql(this.Configuration.GetConnectionString("PostgreSql"))
            );
            services.AddDbContext<AccountContext>(options =>
                options.UseNpgsql(this.Configuration.GetConnectionString("PostgreSql"))
            );

            services.AddHttpClient();
            //ע�������֤
            services.AddIdentityCore<UserInfo>(options =>
            {
                //�������밲ȫ�ԣ����������ɴ�����UserInfoʵ����ģ����ͨ��ע������ϸ����
                options.Password.RequiredLength = 6;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredUniqueChars = 0;

                options.User.RequireUniqueEmail = true;
            })
                    .AddSignInManager<SignInManager<UserInfo>>()
                    .AddUserManager<AccountManager>()
                    .AddUserStore<AccountStore>()
                    .AddEntityFrameworkStores<AccountContext>();


            services.AddHttpContextAccessor();

            //����json web token ����¼��֤����,��ֹCookie������Cookie��ͻ
            services.AddAuthentication(options =>
             {
                 options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                 options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                 options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
                 options.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;
             }
            ).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,//�Ƿ���֤Issuer
                    ValidateAudience = true,//�Ƿ���֤Audience
                    ValidateLifetime = true,//�Ƿ���֤ʧЧʱ��
                    ClockSkew = TimeSpan.FromSeconds(30),
                    ValidateIssuerSigningKey = true,//�Ƿ���֤SecurityKey
                    ValidAudience = this.Configuration.GetValue<string>("Domain"),//Audience
                    ValidIssuer = this.Configuration.GetValue<string>("Domain"),//Issuer���������ǰ��ǩ��jwt������һ��
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.Configuration.GetValue<string>("SecurityKey")))//�õ�SecurityKey
                };
            });

            services.AddSingleton<TokenManager>();

            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration => configuration.RootPath = "mukaiMusic/dist/muKaiMusic");
            //����ڴ滺�浽DI����
            services.AddMemoryCache();

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

            })
                .AddJsonOptions(configure =>
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
            //�������þ�̬��Դ�ļ�
            app.UseStaticFiles();
            app.UseOpenApi();
            //ʹ��swagger
            app.UseSwaggerUi3();
            //���õ�ҳ�澲̬��Դ�ļ�
            if (!env.IsDevelopment())
            {
                app.UseSpaStaticFiles();
            }
            app.UseRouting();

            //�����֤
            app.UseAuthentication();
            //Ȩ����֤
            app.UseAuthorization();
            //�������
            app.UseCors(builder =>
            {
                builder.AllowAnyOrigin();
                builder.AllowAnyMethod();
                builder.AllowAnyHeader();
            });
            app.UseApiCacheMiddleware(Configuration);


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });
            app.UseSpaStaticFiles();
            if (env.IsProduction())
            {
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
