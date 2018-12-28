using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fate.Admin.Extensions
{
    public static class ServiceConfiguration
    {
        public static IServiceProvider ServiceProvider { get; set; }
        public static IServiceCollection ConfigServies(this IServiceCollection service)
        {
            //此处注册服务
            //service.AddScoped<>
            return service;
        }
    }
}
