using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Fate.GateWay
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
           .ConfigureAppConfiguration((hostingContext, builder) => {
               builder
               .SetBasePath(hostingContext.HostingEnvironment.ContentRootPath)
               .AddJsonFile("Ocelot.json");
           })
            .UseUrls("http://+:80")
                .UseStartup<Startup>();
    }
}
