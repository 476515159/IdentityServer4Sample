using Fate.Project.Infrastructure.RedisCache;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class CsRedisServiceCollectionExtensions
    {
        public static IServiceCollection AddCsRedis(this IServiceCollection services, Action<CsRedisOptions> setupAction)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (setupAction == null)
            {
                throw new ArgumentNullException(nameof(setupAction));
            }

            services.AddOptions();
            services.Configure(setupAction);
            //services.Add(ServiceDescriptor.Singleton<IDistributedCache, ServiceStackRedisCache>());
            services.Add(ServiceDescriptor.Singleton(typeof(CsRedisHelper)));

            return services;
        }

        public static IServiceCollection AddCsRedis(this IServiceCollection services, IConfigurationSection section)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (section == null)
            {
                throw new ArgumentNullException(nameof(section));
            }

            services.Configure<CsRedisOptions>(section);

            services.Add(ServiceDescriptor.Singleton(typeof(CsRedisHelper)));

            return services;
        }
    }
}
