using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Fate.Client.HttpClientApi;
using Fate.Client.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace Fate.Client
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

            #region 授权配置
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = "oidc";
            })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            {

                options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
                options.Cookie.Name = CookieAuthenticationDefaults.AuthenticationScheme;//监控浏览器Cookies不难发现有这样一个 .AspNetCore.Cookies 记录了加密的授权信息 

            })
                .AddOpenIdConnect("oidc", options =>
                {
                    options.SignInScheme = "Cookies";
                    options.Authority = "http://localhost:8009";
                    options.ClientId = "testclient2";
                    options.ClientSecret = "secret";
                    options.Scope.Add("Fate_Admin");
                    options.ResponseType = OpenIdConnectResponseType.IdTokenToken;//服务端为简单模式不能有code,混合模式就选择有code的（CodeIdToken）
                    options.GetClaimsFromUserInfoEndpoint = true;//布尔值来设置处理程序是否应该转到用户信息端点检索。额外索赔或不在id_token创建一个身份收到令牌端点。默认为“false”
                    options.RequireHttpsMetadata = false;
                    options.SaveTokens = true;
                });
            #endregion

            #region HttpClient服务
            services.AddHttpContextAccessor();
            services.AddHttpClient();
            services.AddHttpClient("identity", client =>
            {
                client.BaseAddress = new Uri("http://localhost:8009");
            });
            services.AddSingleton(typeof(HttpClientService));
            #endregion

            #region Redis注册
            //-----------------微软的
            services.AddDistributedRedisCache(options =>
            {
                Configuration.GetSection("RedisCache").Bind(options);
                ////用于连接Redis的配置  
                //options.Configuration = Configuration["RedisCache:RedisConnectionString"];
                ////Redis实例名
                //options.InstanceName = Configuration["RedisCache:InstanceName"];
            });


            services.AddSession(options =>
            {
                options.Cookie.Name = "Fate.Session";
                options.IdleTimeout = TimeSpan.FromSeconds(10);//设置session的过期时间
                options.Cookie.HttpOnly = true;//设置在浏览器不能通过js获得该cookie的值
            }
            );
            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseAuthentication();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseSession();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
