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
using Microsoft.AspNetCore.Authentication.Cookies;
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

            #region ��������
            string _migrationAssablyName = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
            services.AddOptions();
            services.Configure<SqlAppSetting>(Configuration.GetSection("ConnectionStrings"));
            services.Configure<Models.Dtos.ServiceDisvoveryOptions>(Configuration.GetSection("ServiceDiscovery"));
            #endregion
            services.AddIdentityServer(idroptions =>
            {
                //�û�����ҳ�涨�����ô���
                idroptions.UserInteraction = new IdentityServer4.Configuration.UserInteractionOptions()
                {
                    LoginUrl = "/Account/Login",//���ر�����¼��ַ  
                    LogoutUrl = "/Account/Logout",//���ر����˳���ַ 
                    ConsentUrl = "/Consent/Index",//���ر���������Ȩͬ��ҳ���ַ
                    ErrorUrl = "/Account/Error", //���ر�������ҳ���ַ
                    LoginReturnUrlParameter = "returnUrl",//���ر������ô��ݸ���¼ҳ��ķ���URL���������ơ�Ĭ��ΪreturnUrl 
                    LogoutIdParameter = "logoutId", //���ر������ô��ݸ�ע��ҳ���ע����ϢID���������ơ�ȱʡΪlogoutId 
                    ConsentReturnUrlParameter = "returnUrl", //���ر������ô��ݸ�ͬ��ҳ��ķ���URL���������ơ�Ĭ��ΪreturnUrl
                    ErrorIdParameter = "errorId", //���ر������ô��ݸ�����ҳ��Ĵ�����ϢID���������ơ�ȱʡΪerrorId
                    CustomRedirectReturnUrlParameter = "returnUrl", //���ر������ô���Ȩ�˵㴫�ݸ��Զ����ض���ķ���URL���������ơ�Ĭ��ΪreturnUrl
                    CookieMessageThreshold = 5 //���ر��������������Cookie�Ĵ�С�����ƣ�����Cookies���������ƣ���Ч�ı�֤��������򿪶��ѡ���һ��������Cookies���ƾͻ������ǰ��Cookiesֵ

                };
            }
                )

                //.AddExtensionGrantValidator<SmsAuthCodeValidator>()
                //.AddSecretValidator<AuthSecretValidator>() //�ͻ���ģʽ
                .AddResourceOwnerValidator<ResourceOwnerPasswordValidator>()//����ģʽ
                .AddDeveloperSigningCredential()
             //.AddInMemoryClients(Config.GetClients())
             //.AddInMemoryIdentityResources(Config.GetIdentityResource())
             //.AddInMemoryApiResources(Config.GetApiResources());
            #region �������ݿ�
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
                     //UseMySql��Ҫʹ��Pomelo.EntityFrameworkCore.MySql�µ�,ʹ��mySql.data.entityframework�ᱨ
                     // The 'MySQLNumberTypeMapping' does not support value conversions ����
                     build.UseMySql(Configuration["IdentityServerConnections"], Mysqloptions =>
                     {
                         Mysqloptions.MigrationsAssembly(_migrationAssablyName);
                         Mysqloptions.UseRelationalNulls();
                     });
                     options.EnableTokenCleanup = true; //�����Token������
                     options.TokenCleanupInterval = 1800;//��������ʱ��Secends
                 };
             })
             .AddProfileService<ProfileService>();
            #endregion


            #region ��Ȩ����
            //JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = "oidc";
            })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            {

                options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
                options.Cookie.Name = CookieAuthenticationDefaults.AuthenticationScheme;//��������Cookies���ѷ���������һ�� .AspNetCore.Cookies ��¼�˼��ܵ���Ȩ��Ϣ 

            });
            #endregion

            #region ����ע��

            services.AddSingleton<IDnsQuery>(p =>
            {
                var serviceOptions = Configuration.GetSection("ServiceDiscovery").Get<Models.Dtos.ServiceDisvoveryOptions>();
                var client = new LookupClient(serviceOptions.Consul.DnsEndpoint.ToIPEndPoint());
                return client;
            });

            //ע��ȫ�ֵ���ResilienceClientFactory
            services.AddSingleton(typeof(ResilienceClientFactory), sp =>
            {
                var logger = sp.GetRequiredService<ILogger<ResilienceHttpClient>>();
                var httpContextAccessor = sp.GetRequiredService<IHttpContextAccessor>();
                string retryCount = Configuration["Policy:retryCount"];
                string exceptionCountAllowedBeforeBreaking = Configuration["Policy:exceptionCountAllowedBeforeBreaking"];
                return new ResilienceClientFactory(logger, httpContextAccessor, int.Parse(retryCount), int.Parse(exceptionCountAllowedBeforeBreaking));
            });


            //services.AddSingleton(new HttpClient(new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip }));
            //ע��ȫ�ֵ���IhttpClient
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
            app.RegisterZipkin(applicationLifetime, loggerFactory);//����identityǰ
            app.UseIdentityServer();
            app.InitData();//��ʼ��identity����

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
