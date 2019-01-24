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
            #region ��������
            services.AddMvc()
                //���շ��ص�json�������õ������Ĵ�Сд��ʽ������
                .AddJsonOptions(options => { options.SerializerSettings.ContractResolver = new DefaultContractResolver(); });
            //���ѡ��
            services.AddOptions();
            //��������Ϣ����DIע��
            services.Configure<AppSetting>(Configuration.GetSection("SiteConfig"));
            var a = Configuration.GetSection("SiteConfig").Get<AppSetting>();
            var connetc = Configuration.GetConnectionString("DefaultConnection");
            #endregion

            #region ��Ȩ����
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = "oidc";
            })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options => {
                options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
                options.Cookie.Name = CookieAuthenticationDefaults.AuthenticationScheme;//��������Cookies���ѷ���������һ�� .AspNetCore.Cookies ��¼�˼��ܵ���Ȩ��Ϣ 
            })
                .AddOpenIdConnect("oidc", options =>
                 {
                     options.SignInScheme = "Cookies";//������OpenID ConnectЭ����ɺ�ʹ��cookie������򷢳�cookie
                     options.Authority = "http://localhost:8009";
                     options.ClientId = "testadmin2";
                     options.ClientSecret = "secret";
                     options.Scope.Add("Fate_Admin");
                     options.ResponseType = OpenIdConnectResponseType.IdTokenToken;////�����Ϊ��ģʽ������code,���ģʽ��ѡ����code�ģ�CodeIdToken��
                     options.GetClaimsFromUserInfoEndpoint = true;//����ֵ�����ô�������Ƿ�Ӧ��ת���û���Ϣ�˵�����������������id_token����һ������յ����ƶ˵㡣Ĭ��Ϊ��false��
                     options.RequireHttpsMetadata = false;
                     options.SaveTokens = true;
                 });
            #endregion

            //services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            //.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme);

            #region ����ע��
            services.ConfigServies();
            #endregion
            //services.AddSQLSugarClient
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            //��¼ʱ��
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
