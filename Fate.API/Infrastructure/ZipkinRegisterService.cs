using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using zipkin4net;
using zipkin4net.Middleware;
using zipkin4net.Tracers.Zipkin;
using zipkin4net.Transport.Http;

namespace Fate.API.Infrastructure
{
    public static class ZipkinRegisterService
    {
        public static void RegisterZipkin(this IApplicationBuilder app, IApplicationLifetime applicationLifetime, ILoggerFactory loggerFactory)
        {
            applicationLifetime.ApplicationStarted.Register(() =>
            {
                TraceManager.SamplingRate = 1.0f;//记录数据密度,1.0表示全部记录
                var logger = new TracingLogger(loggerFactory, "zipkin4net");
                var httpsender = new HttpZipkinSender("http://localhost:9411", "application/json");//服务器地址
                var tracer = new ZipkinTracer(httpsender, new JSONSpanSerializer(),new Statistics());//注册zipkin
                TraceManager.RegisterTracer(tracer);
                TraceManager.Start(logger);//放到内存中的数据
            });
            applicationLifetime.ApplicationStopped.Register(() => TraceManager.Stop());
            app.UseTracing("UserApi");//记录微服务名称 唯一性
        }
    }
}
