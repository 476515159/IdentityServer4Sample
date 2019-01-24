using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Fate.Admin.Extensions;
using Fate.Admin.Middleware;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Models;
using Newtonsoft.Json.Serialization;
using SqlSugar;

namespace Fate.Admin
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
            #region 基础配置
            services.AddMvc()
                //按照返回的json数据设置的正常的大小写格式来接收
                .AddJsonOptions(options => { options.SerializerSettings.ContractResolver = new DefaultContractResolver(); });
            //添加选项
            services.AddOptions();
            //将配置信息进行DI注入
            services.Configure<AppSetting>(Configuration.GetSection("SiteConfig"));
            var a = Configuration.GetSection("SiteConfig").Get<AppSetting>();
            var connetc = Configuration.GetConnectionString("DefaultConnection");
            #endregion

            #region 授权配置
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = "oidc";
            })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options => {
                options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
                options.Cookie.Name = CookieAuthenticationDefaults.AuthenticationScheme;//监控浏览器Cookies不难发现有这样一个 .AspNetCore.Cookies 记录了加密的授权信息 
            })
                .AddOpenIdConnect("oidc", options =>
                 {
                     options.SignInScheme = "Cookies";//用于在OpenID Connect协议完成后使用cookie处理程序发出cookie
                     options.Authority = "http://localhost:8009";
                     options.ClientId = "testadmin2";
                     options.ClientSecret = "secret";
                     options.Scope.Add("Fate_Admin");
                     options.ResponseType = OpenIdConnectResponseType.IdTokenToken;////服务端为简单模式不能有code,混合模式就选择有code的（CodeIdToken）
                     options.GetClaimsFromUserInfoEndpoint = true;//布尔值来设置处理程序是否应该转到用户信息端点检索。额外索赔或不在id_token创建一个身份收到令牌端点。默认为“false”
                     options.RequireHttpsMetadata = false;
                     options.SaveTokens = true;
                 });
            #endregion

            //services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            //.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme);

            #region 服务注册
            services.ConfigServies();
            #endregion
            //services.AddSQLSugarClient
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            //记录时间
            app.UseCalculateExecutionTime();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }
            //app.UseSession();
            app.UseAuthentication();
            
            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action=Index}/{id?}");
            });
        }
    }
}
