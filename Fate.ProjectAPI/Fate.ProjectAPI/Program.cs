using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Fate.ProjectAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((context, config) =>
            {
                var env = context.HostingEnvironment;
                config.SetBasePath(env.ContentRootPath)
                //使用AddJsonFile方法把项目中的appsettings.json配置文件加载进来，后面的reloadOnChange顾名思义就是文件如果改动就重新加载
                .AddJsonFile("appsettings.json", true, true)

                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            })
            .UseUrls("https://localhost:8005")//https
                .UseStartup<Startup>();
    }
}
