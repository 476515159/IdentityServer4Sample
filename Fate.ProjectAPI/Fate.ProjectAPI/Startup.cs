using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MediatR;
using System.Reflection;
using Models;
using System.IdentityModel.Tokens.Jwt;
using Fate.Project.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Fate.Project.Domain.AggregatesModel;
using Fate.Project.Infrastructure.Repositories;
using Fate.Project.Domain;
using Consul;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http.Features;
using Fate.ProjectAPI.Applications.Queries;
using Fate.ProjectAPI.Applications.Services;
using Fate.Project.Infrastructure.RedisCache;

namespace Fate.ProjectAPI
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
            services.AddOptions();
            #region 授权
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.Authority = "http://localhost";
                    options.Audience = "Fate_ProjectAPI";
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                });
            #endregion

            #region 数据库配置
            services.AddDbContext<ProjectContext>(optiosbuilder =>
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

            #region consul服务注册
            services.AddSingleton<IConsulClient>(p => new ConsulClient(cfg =>
            {
                var serviceConfiguration = p.GetRequiredService<IOptions<Models.Dtos.ServiceDisvoveryOptions>>().Value;

                if (!string.IsNullOrEmpty(serviceConfiguration.Consul.HttpEndpoint))
                {
                    // if not configured, the client will use the default value "127.0.0.1:8500"
                    cfg.Address = new Uri(serviceConfiguration.Consul.HttpEndpoint);
                }
            }));
            #endregion

            #region Cap 和RabbitMq注册
            var capConsulConfig = Configuration.GetSection("CapConsulConfig").Get<CapConsulConfig>();
            //string UserConnectionStrings =  Configuration["UserConnectionStrings"];
            services.AddCap(options =>
            {
                options.UseEntityFramework<ProjectContext>()
                //options.UseMySql(mysql =>
                //{
                //    mysql.ConnectionString = UserConnectionStrings;
                //    mysql.TableNamePrefix
                //})
               // .UseRabbitMQ("localhost")
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

            #region CsRedis注册
            services.Configure<CsRedisOptions>(Configuration.GetSection("CsRedis"));
            services.AddSingleton<CsRedisHelper>();
            #endregion

            #region DI注入
            services.AddMediatR();//同一个程序集不用加参数
            //services.AddMediatR(typeof(Fate.Project.Domain.AggregatesModel.Project).GetType().Assembly);

            services.AddSingleton<IUnitOfWork, ProjectContext>();
            services.AddScoped<IProjectRepository, ProjectRepository>();
            services.AddScoped<IRecommendService, RecommendService>();
            services.AddScoped<IProjectQueries, ProjectQueries>();
            #endregion

            //services.addmed
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory
            , IApplicationLifetime applicationLifetime, IOptions<Models.Dtos.ServiceDisvoveryOptions> serviceOptions
            , IConsulClient consul)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }
            loggerFactory.AddDebug();
            #region Consul 注册与停止
            //启动时注册服务
            //applicationLifetime.ApplicationStarted.Register(() =>
            //{
            //    RegisterService(app, serviceOptions, consul, applicationLifetime);
            //});

            ////停止时销毁服务
            //applicationLifetime.ApplicationStopping.Register(() =>
            //{
            //    DeRegisterService(app, serviceOptions, consul);
            //});

            #endregion
            app.UseAuthentication();
            app.UseCap();
            app.UseStaticFiles();
            app.UseHttpsRedirection();
            
            app.UseMvc();
        }

        #region Consul 注册与停止
        private void RegisterService(IApplicationBuilder app, IOptions<Models.Dtos.ServiceDisvoveryOptions> serviceOptions
            , IConsulClient consul, IApplicationLifetime applicationLifetime)
        {
            //获取当前运行的地址
            var features = app.Properties["server.Features"] as FeatureCollection;
            var addresses = features.Get<IServerAddressesFeature>()
                .Addresses
                .Select(p => new Uri(p));

            foreach (var address in addresses)
            {
                var serviceId = $"{serviceOptions.Value.ServiceName}_{address.Host}:{address.Port}";

                var httpCheck = new AgentServiceCheck()
                {
                    DeregisterCriticalServiceAfter = TimeSpan.FromMinutes(1),
                    Interval = TimeSpan.FromSeconds(30),
                    HTTP = new Uri(address, "HealthCheck").OriginalString
                };

                var registration = new AgentServiceRegistration
                {
                    Checks = new[] { httpCheck },
                    Address = address.Host,
                    ID = serviceId,
                    Name = serviceOptions.Value.ServiceName,
                    Port = address.Port
                };

                consul.Agent.ServiceRegister(registration).GetAwaiter().GetResult();
            }
        }

        private void DeRegisterService(IApplicationBuilder app, IOptions<Models.Dtos.ServiceDisvoveryOptions> serviceOptions
            , IConsulClient consul)
        {
            //获取当前运行的地址
            var features = app.Properties["server.Features"] as FeatureCollection;
            var addresses = features.Get<IServerAddressesFeature>()
                .Addresses
                .Select(p => new Uri(p));

            foreach (var address in addresses)
            {
                var serviceId = $"{serviceOptions.Value.ServiceName}_{address.Host}:{address.Port}";

                consul.Agent.ServiceDeregister(serviceId).GetAwaiter().GetResult();
            }
        }
        #endregion
    }
}
