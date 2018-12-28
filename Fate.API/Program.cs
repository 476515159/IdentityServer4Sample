using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Fate.API
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
    .ConfigureAppConfiguration((context, config) =>
    {
        var env = context.HostingEnvironment;
        config.SetBasePath(env.ContentRootPath)
        //使用AddJsonFile方法把项目中的appsettings.json配置文件加载进来，后面的reloadOnChange顾名思义就是文件如果改动就重新加载
        .AddJsonFile("appsettings.json", true, true)

        .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
        .AddEnvironmentVariables();

    })
            .UseUrls("http://localhost:8001")
        //.UseIISIntegration()//采用IIS进行发布
        .Build();
    }
}
