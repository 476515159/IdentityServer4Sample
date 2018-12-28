using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Fate.Identity
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
            .UseKestrel()//跨平台 ASP.NET Core Web 服务器，它基于 libuv（一个跨平台异步 I/O 库）。 Kestrel 是 Web 服务器，默认包括在 ASP.NET Core 项目模板中。 
            .UseUrls("http://localhost:8009")
                //.UseIISIntegration() 
                .UseStartup<Startup>();
    }
}
