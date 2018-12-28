using Resilience;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Polly;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Polly.Wrap;
using System.Net.Http;

namespace Fate.ContactAPI.Infrastructure
{
    public class ResilienceClientFactory
    {

        private readonly ILogger<ResilienceHttpClient> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        //重试次数
        private int _retryCount;
        //熔断之前允许的异常次数
        private int _exceptionCountAllowedBeforeBreaking;

        public ResilienceClientFactory(ILogger<ResilienceHttpClient> logger, 
            IHttpContextAccessor httpContextAccessor,
            int retryCount,
            int exceptionCountAllowedBeforeBreaking)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _retryCount = retryCount;
            _exceptionCountAllowedBeforeBreaking = exceptionCountAllowedBeforeBreaking;
        }


        public ResilienceHttpClient GetResilienceHttpClient => 
            new ResilienceHttpClient("contactApi",origin=> CreatePolicy(origin),_logger,_httpContextAccessor);

        //失败的重试和熔断
        private Policy[] CreatePolicy(string origin)
        {
            return new Policy[] {
                Policy.Handle<HttpRequestException>()//定义条件
                .WaitAndRetryAsync(_retryCount,
                retryAttempt=>TimeSpan.FromSeconds(Math.Pow(2,retryAttempt)),
                (exception,timespan,retryCount,context)=>{
                    var msg=$"第{retryCount}次重试"+
                    $"of {context.PolicyKey}"+
                    $"at {context.OperationKey}"+
                    $"due to:{exception}";

                    _logger.LogWarning(msg);
                    _logger.LogDebug(msg);
                }),
                Policy.Handle<HttpRequestException>()
                .CircuitBreakerAsync(_exceptionCountAllowedBeforeBreaking,TimeSpan.FromMinutes(1),
                (exception,duration)=>{
                    _logger.LogTrace("熔断器打开");
                },()=>{
                    _logger.LogTrace("熔断器关闭");
                })
                
            };
        }
    }
}
