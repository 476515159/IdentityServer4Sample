using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Consul;
using DnsClient;
using Fate.ContactAPI.Data;
using Fate.ContactAPI.Infrastructure;
using Fate.ContactAPI.IntegrationEvents.EventHanding;
using Fate.ContactAPI.Services;
using Fate.EfContext;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Models;
using Resilience;
using MongoDB.Driver;

namespace Fate.ContactAPI
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

            services.AddMvc(options =>
            {
                options.Filters.Add(typeof(GlobalExceptionFilter));
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.Configure<AppSetting>(Configuration.GetSection("MongoDb"));
            services.Configure<Models.Dtos.ServiceDisvoveryOptions>(Configuration.GetSection("ServiceDiscovery"));

            #region 授权
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.Authority = "http://localhost";
                    options.Audience = "Fate_ContactAPI";
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                });
            #endregion

            #region MongoDb数据库配置
            //string _migrationAssablyName = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
            //services.AddDbContext<EntityDbContext>(optiosbuilder =>
            //{
            //    var _userappsetting = Configuration.GetSection("ConnectionStrings").Get<SqlAppSetting>();
            //    if (_userappsetting == null)
            //    {
            //        throw new Exception("数据库连接字符串未配置");
            //    }

            //    switch (_userappsetting.DbType)
            //    {
            //        case 1:
            //            optiosbuilder.UseSqlServer(_userappsetting.SqlConnection, sqlserver =>
            //            {
            //                sqlserver.MigrationsAssembly(_migrationAssablyName);
            //                sqlserver.UseRelationalNulls();
            //                sqlserver.UseRowNumberForPaging();

            //            });
            //            break;
            //        default:
            //            optiosbuilder.UseMySQL(_userappsetting.MySqlConnection, mysql =>
            //            {
            //                mysql.MigrationsAssembly(_migrationAssablyName);
            //                mysql.UseRelationalNulls();
            //            });
            //            break;

            //    }
            //});
            #endregion


            #region 注册服务
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

            services.AddHttpContextAccessor();
            services.AddScoped(typeof(ContactContext));
            services.AddScoped<IContactApplyRequestRepository, MongoContactApplyRequestRepository>();
            services.AddScoped<IUserRepository, MongoUserRepository>();
            services.AddScoped<IContactRepository, MongoContactRepository>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped(typeof(UserProfileChangedEventHander));
            #endregion

            #region Cap 和RabbitMq注册
            var capConsulConfig = Configuration.GetSection("CapConsulConfig").Get<CapConsulConfig>();
            var connectionStrings = Configuration.GetSection("ConnectionStrings").Get<SqlAppSetting>();
            services.AddCap(options =>
            {
                options.UseMySql(mysql=> {
                    mysql.ConnectionString = connectionStrings.MySqlConnection;
                })
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

            #region 跨域策略
            //添加Cors，并配置CorsPolicy 
            services.AddCors(options => options.AddPolicy("CorsTest", p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));
            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime applicationLifetime, IOptions<Models.Dtos.ServiceDisvoveryOptions> serviceOptions
            , IConsulClient consul,ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            #region Consul 注册与停止
            //启动时注册服务
            //applicationLifetime.ApplicationStarted.Register(() => {
            //    RegisterService(app, serviceOptions, consul, applicationLifetime);
            //});

            ////停止时销毁服务
            //applicationLifetime.ApplicationStopping.Register(() => {
            //    DeRegisterService(app, serviceOptions, consul);
            //});

            #endregion

            //注意UseCors()要在UseMvc()之前 ，然后Controller或者Action中加上特性[EnableCors("CorsTest")]
            app.UseCors("CorsTest");
            app.UseAuthentication();
            app.UseCap();
            app.RegisterZipkin(applicationLifetime,loggerFactory);
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
