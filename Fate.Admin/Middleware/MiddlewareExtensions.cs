using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fate.Admin.Middleware
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseCalculateExecutionTime(this IApplicationBuilder app)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }
            return app.UseMiddleware<CalculateExecutionTimeMiddleware>();
        }
    }
}
