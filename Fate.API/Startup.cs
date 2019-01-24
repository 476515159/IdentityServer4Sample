using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Consul;
using Fate.API.Extensions;
using Fate.API.Infrastructure;
using Fate.EfContext;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.SqlServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Models;

namespace Fate.API
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
            services.AddMvc(options =>
            {
                options.Filters.Add(typeof(GlobalExceptionFilter));
            });
            services.AddOptions();
            services.Configure<SqlAppSetting>(Configuration.GetSection("ConnectionStrings"));
            services.Configure<AppSetting>(Configuration.GetSection("SiteConfig"));
            services.Configure<Models.Dtos.ServiceDisvoveryOptions>(Configuration.GetSection("ServiceDiscovery"));
            //var a = Configuration.GetSection("SiteConfig").Get<AppSetting>();
            //var connetc = Configuration.GetConnectionString("MySqlConnection");
            //var devcon = Configuration["ConnectionStrings:SqlConnection"];
            //var devcon2 = Configuration["SiteConfig:Name"];

            #endregion

            #region 授权
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.Authority = "http://localhost";
                    options.Audience = "Fate_UserAPI";
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                });
            #endregion

            #region 数据库配置
            services.AddDbContext<EntityDbContext>(optiosbuilder =>
            {
                var _userappsetting = Configuration.GetSection("ConnectionStrings").Get<SqlAppSetting>();
                if (_userappsetting == null)
                {
                    throw new Exception("数据库连接字符串未配置");
                }

                switch (_userappsetting.DbType)
                {
                    case 1:
                        optiosbuilder.UseSqlServer(_userappsetting.SqlConnection, sqlserver =>
                        {
                            sqlserver.MigrationsAssembly(_migrationAssablyName);
                            sqlserver.UseRelationalNulls();
                            sqlserver.UseRowNumberForPaging();

                        });
                        break;
                    default:
                        optiosbuilder.UseMySQL(_userappsetting.MySqlConnection, mysql =>
                        {
                            mysql.MigrationsAssembly(_migrationAssablyName);
                            mysql.UseRelationalNulls();
                        });
                        break;

                }
            });
            #endregion

            #region Cap 和RabbitMq注册
            var capConsulConfig = Configuration.GetSection("CapConsulConfig").Get<CapConsulConfig>();
            services.AddCap(options =>
            {
                options.UseEntityFramework<EntityDbContext>()
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

            #region Swagger 配置
            services.AddSwaggerGen(c =>{
                c.SwaggerDoc("v1", new Swashbuckle.AspNetCore.Swagger.Info { Title = "Fate.API",Version="1.0" });
                //var xml = Path.Combine(AppContext.BaseDirectory, "Common\\Fate.API.xml");
                var xml = Path.Combine(Directory.GetCurrentDirectory(), "Common\\Fate.API.xml");
                c.IncludeXmlComments(xml);
            });
            #endregion

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

            #region Consul 注册与停止
            //启动时注册服务
            applicationLifetime.ApplicationStarted.Register(() => {
                RegisterService(app,serviceOptions,consul,applicationLifetime);
            });

            //停止时销毁服务
            applicationLifetime.ApplicationStopping.Register(() => {
                DeRegisterService(app, serviceOptions, consul);
            });

            #endregion

            loggerFactory.AddDebug();

            app.UseAuthentication();

            app.CheckMigrations();

            app.UseCap();//2.3版本移除了 无需注册

            app.RegisterZipkin(applicationLifetime,loggerFactory);
            #region Swagger 配置
            //启用中间件服务生成Swagger作为JSON终结点
            app.UseSwagger(c => { c.RouteTemplate = "swagger/{documentName}/swagger.json"; });
            //启用中间件服务对swagger-ui，指定Swagger JSON终结点
            app.UseSwaggerUI(c =>
            {
                //c.RoutePrefix = "swagger/ui";
                c.SwaggerEndpoint("v1/swagger.json", "Fate.API");
                //c.OAuth2RedirectUrl("http://localhost:5000");
                //c.OAuthClientId ("mvc");
                //c.OAuthAppName("Fate_GateWay");
                //c.OAuthClientSecret("secret");
                //c.OAuthUseBasicAuthenticationWithAccessCodeGrant();
            });
            #endregion
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