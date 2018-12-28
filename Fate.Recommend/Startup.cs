using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Fate.Recommend.Data;
using Fate.Recommend.IntegrationEvents.EventHanding;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Models;

namespace Fate.Recommend
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
            string _migrationAssablyName = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
            services.Configure<SqlAppSetting>(Configuration.GetSection("ConnectionStrings"));
            services.Configure<Models.Dtos.ServiceDisvoveryOptions>(Configuration.GetSection("ServiceDiscovery"));
            var connectionStrings = Configuration.GetSection("ConnectionStrings").Get<SqlAppSetting>();
            #endregion
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            #region 授权
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.Authority = "http://localhost";
                    options.Audience = "Fate_RecommendAPI";
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                });
            #endregion

            #region 数据库配置
            services.AddDbContext<RecommendContext>(optiosbuilder =>
            {
                //var _userappsetting = Configuration.GetSection("ConnectionStrings").Get<SqlAppSetting>();
                if (connectionStrings == null)
                {
                    throw new Exception("数据库连接字符串未配置");
                }

                switch (connectionStrings.DbType)
                {
                    case 1:
                        optiosbuilder.UseSqlServer(connectionStrings.SqlConnection, sqlserver =>
                        {
                            sqlserver.MigrationsAssembly(_migrationAssablyName);
                            sqlserver.UseRelationalNulls();
                            sqlserver.UseRowNumberForPaging();

                        });
                        break;
                    default:
                        optiosbuilder.UseMySQL(connectionStrings.MySqlConnection, mysql =>
                        {
                            mysql.MigrationsAssembly(_migrationAssablyName);
                            mysql.UseRelationalNulls();
                        });
                        break;

                }
            });
            #endregion

            #region DI注入
            services.AddScoped(typeof(ProjectCreatedHander));
            #endregion

            #region Cap 和RabbitMq注册
            var capConsulConfig = Configuration.GetSection("CapConsulConfig").Get<CapConsulConfig>();
            string UserConnectionStrings = Configuration["UserConnectionStrings"];
            services.AddCap(options =>
            {
                options.UseEntityFramework<RecommendContext>()
                //options.UseMySql(mysql =>
                //{
                //    mysql.ConnectionString = UserConnectionStrings;
                //})
                //.UseRabbitMQ("localhost")
                .UseRabbitMQ(mq =>
                {
                    mq.HostName = "192.168.0.73";
                    mq.UserName = "lijinlong";
                    mq.Password = "123";
                })
                .UseDashboard()//启用仪表盘界面
                .UseDiscovery(discovery =>
                {
                    discovery.CurrentNodeHostName = capConsulConfig.CurrentNodeHostName;
                    discovery.CurrentNodePort = capConsulConfig.CurrentNodePort;
                    discovery.DiscoveryServerHostName = capConsulConfig.DiscoveryServerHostName;
                    discovery.DiscoveryServerPort = capConsulConfig.DiscoveryServerPort;
                    discovery.NodeId = capConsulConfig.NodeId;
                    discovery.NodeName = capConsulConfig.NodeName;
                    //discovery.MatchPath = capConsulConfig.MatchPath;
                });
                
            });
            #endregion

            

            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            loggerFactory.AddDebug();
            app.UseCap();
            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
