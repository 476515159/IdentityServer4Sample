using Fate.EfContext;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fate.API.Extensions
{
    public static class DbcontextExtensions
    {
        /// <summary>
        /// 检查是否需要迁移
        /// </summary>
        /// <param name="app"></param>
        public static void CheckMigrations(this IApplicationBuilder app)
        {
            using (var scopes=app.ApplicationServices.CreateScope())
            {
                var services = scopes.ServiceProvider.GetRequiredService<EntityDbContext>();
                if (services.Database.GetPendingMigrations().Any())//判断是否有待迁移
                {
                    //执行迁移
                    services.Database.Migrate();
                }
            }
        }
    }
}
