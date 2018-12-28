using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Fate.Admin.Middleware
{
    public class CalculateExecutionTimeMiddleware
    {
        private readonly RequestDelegate _next;//下一个中间件
        private readonly ILogger _logger;
        Stopwatch stopwatch;
        public CalculateExecutionTimeMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            if (loggerFactory == null)
            {
                throw new ArgumentNullException(nameof(loggerFactory));
            }
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _logger = loggerFactory.CreateLogger<CalculateExecutionTimeMiddleware>();
        }

        public async Task Invoke(HttpContext context)
        {
            stopwatch = Stopwatch.StartNew();
            //stopwatch.Start();//在下一个中间价处理前，启动计时器
            await _next.Invoke(context);

            stopwatch.Stop();//所有的中间件处理完后，停止秒表。
            _logger.LogInformation($@"接口{context.Request.Path}耗时{stopwatch.ElapsedMilliseconds}ms");
        }
    }
}
