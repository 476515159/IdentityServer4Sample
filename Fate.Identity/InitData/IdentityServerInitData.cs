using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fate.Identity.InitData
{
    public static class IdentityServerInitData
    {
        public static void InitData(this IApplicationBuilder app)
        {
            using (var scopes = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                scopes.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();
                var context = scopes.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                context.Database.Migrate();
                if (!context.Clients.Any())
                {
                    foreach (var client in Config.GetClients())
                    {
                        context.Clients.Add(client.ToEntity());
                    }
                    context.SaveChanges();
                }
                if (!context.ApiResources.Any())
                {
                    foreach (var api in Config.GetApiResources())
                    {
                        context.ApiResources.Add(api.ToEntity());
                    }
                    context.SaveChanges();
                }
                if (!context.IdentityResources.Any())
                {
                    foreach (var identity in Config.GetIdentityResource())
                    {
                        context.IdentityResources.Add(identity.ToEntity());
                    }
                    context.SaveChanges();
                }
            }
        }
    }
}
