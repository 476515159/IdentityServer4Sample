using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Fate.Admin
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
            //添加autofac
            .ConfigureServices(service=> { service.AddAutofac(); })
            //配置appsettings配置文件
            .ConfigureAppConfiguration((context, config) =>
            {
                var env = context.HostingEnvironment;
                config.SetBasePath(env.ContentRootPath)
                //使用AddJsonFile方法把项目中的appsettings.json配置文件加载进来，后面的reloadOnChange顾名思义就是文件如果改动就重新加载
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            })

            //.UseIISIntegration()//采用IIS进行发布
            .UseUrls("http://localhost:8008")
            .UseKestrel()
                .Build();
    }
}
