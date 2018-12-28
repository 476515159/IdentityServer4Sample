using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using DnsClient;
using Fate.EfContext;
using Fate.Identity.Authentication;
using Fate.Identity.Infrastructure;
using Fate.Identity.InitData;
using Fate.Identity.Services;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Models;
using Resilience;
using zipkin4net.Middleware;


namespace Fate.Identity
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });


            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            #region 基础配置
            string _migrationAssablyName = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
            services.AddOptions();
            services.Configure<SqlAppSetting>(Configuration.GetSection("ConnectionStrings"));
            services.Configure<Models.Dtos.ServiceDisvoveryOptions>(Configuration.GetSection("ServiceDiscovery"));
            #endregion
            services.AddIdentityServer(idroptions =>
            {
                //用户交互页面定向设置处理
                idroptions.UserInteraction = new IdentityServer4.Configuration.UserInteractionOptions()
                {
                    LoginUrl = "/Account/Login",//【必备】登录地址  
                    LogoutUrl = "/Account/Logout",//【必备】退出地址 
                    ConsentUrl = "/Consent/Index",//【必备】允许授权同意页面地址
                    ErrorUrl = "/Account/Error", //【必备】错误页面地址
                    LoginReturnUrlParameter = "returnUrl",//【必备】设置传递给登录页面的返回URL参数的名称。默认为returnUrl 
                    LogoutIdParameter = "logoutId", //【必备】设置传递给注销页面的注销消息ID参数的名称。缺省为logoutId 
                    ConsentReturnUrlParameter = "returnUrl", //【必备】设置传递给同意页面的返回URL参数的名称。默认为returnUrl
                    ErrorIdParameter = "errorId", //【必备】设置传递给错误页面的错误消息ID参数的名称。缺省为errorId
                    CustomRedirectReturnUrlParameter = "returnUrl", //【必备】设置从授权端点传递给自定义重定向的返回URL参数的名称。默认为returnUrl
                    CookieMessageThreshold = 5 //【必备】由于浏览器对Cookie的大小有限制，设置Cookies数量的限制，有效的保证了浏览器打开多个选项卡，一旦超出了Cookies限制就会清除以前的Cookies值

                };
            }
                )

                //.AddExtensionGrantValidator<SmsAuthCodeValidator>()
                //.AddSecretValidator<AuthSecretValidator>() //客户端模式
                .AddResourceOwnerValidator<ResourceOwnerPasswordValidator>()//密码模式
                .AddDeveloperSigningCredential()
             //.AddInMemoryClients(Config.GetClients())
             //.AddInMemoryIdentityResources(Config.GetIdentityResource())
             //.AddInMemoryApiResources(Config.GetApiResources());
            #region 生成数据库
             .AddConfigurationStore(options =>
             {
                 options.ConfigureDbContext = builder =>
                 {
                     builder.UseMySql(Configuration["IdentityServerConnections"], Mysqloptions =>
                     {
                         Mysqloptions.MigrationsAssembly(_migrationAssablyName);
                         Mysqloptions.UseRelationalNulls();
                     });
                 };
             })
            .AddOperationalStore(options =>
             {
                 options.ConfigureDbContext = build =>
                 {
                     //UseMySql需要使用Pomelo.EntityFrameworkCore.MySql下的,使用mySql.data.entityframework会报
                     // The 'MySQLNumberTypeMapping' does not support value conversions 错误
                     build.UseMySql(Configuration["IdentityServerConnections"], Mysqloptions =>
                     {
                         Mysqloptions.MigrationsAssembly(_migrationAssablyName);
                         Mysqloptions.UseRelationalNulls();
                     });
                     options.EnableTokenCleanup = true; //允许对Token的清理
                     options.TokenCleanupInterval = 1800;//清理周期时间Secends
                 };
             })
             .AddProfileService<ProfileService>();
            #endregion


            #region 服务注册

            services.AddSingleton<IDnsQuery>(p =>
            {
                var serviceOptions = Configuration.GetSection("ServiceDiscovery").Get<Models.Dtos.ServiceDisvoveryOptions>();
                var client = new LookupClient(serviceOptions.Consul.DnsEndpoint.ToIPEndPoint());
                return client;
            });

            //注册全局单例ResilienceClientFactory
            services.AddSingleton(typeof(ResilienceClientFactory), sp =>
            {
                var logger = sp.GetRequiredService<ILogger<ResilienceHttpClient>>();
                var httpContextAccessor = sp.GetRequiredService<IHttpContextAccessor>();
                string retryCount = Configuration["Policy:retryCount"];
                string exceptionCountAllowedBeforeBreaking = Configuration["Policy:exceptionCountAllowedBeforeBreaking"];
                return new ResilienceClientFactory(logger, httpContextAccessor, int.Parse(retryCount), int.Parse(exceptionCountAllowedBeforeBreaking));
            });


            //services.AddSingleton(new HttpClient(new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip }));
            //注册全局单例IhttpClient
            services.AddSingleton<IhttpClient>(sp =>
            {
                return sp.GetRequiredService<ResilienceClientFactory>().GetResilienceHttpClient;
            });

            services.AddScoped<IAuthCodeServices, AuthCodeServices>()
                .AddScoped<IUserServices, UserServices>();
            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime applicationLifetime
            , ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }
            app.RegisterZipkin(applicationLifetime, loggerFactory);//放在identity前
            app.UseIdentityServer();
            app.InitData();//初始化identity数据

            app.UseStaticFiles();

            app.UseCookiePolicy();
            app.UseAuthentication();

            
            app.UseMvc(router =>
            {
                router.MapRoute(
                   name: "default",
                   template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

    }
}
