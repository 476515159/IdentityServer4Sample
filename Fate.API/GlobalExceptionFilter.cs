using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Models;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace Fate.API
{
    public class GlobalExceptionFilter:IExceptionFilter
    {
        private readonly IHostingEnvironment _env;
        private readonly ILogger<GlobalExceptionFilter> _logs;

        public GlobalExceptionFilter(IHostingEnvironment env, ILogger<GlobalExceptionFilter> logs)
        {
            _env = env;
            _logs = logs;
        }

        public void OnException(ExceptionContext context)
        {
            if (context.Exception.GetType() == typeof(OperationException))
            {
                var json = new JsonErrorResponse {
                    Message = context.Exception.Message
                };
                
                context.Result = new BadRequestObjectResult(json);
            }
            else
            {
                var json = new JsonErrorResponse
                {
                    Message = "发生了未知错误"
                };
                if (_env.IsDevelopment())
                {
                    json.DeveloperMessage = context.ActionDescriptor.DisplayName+ "  "+context.Exception.Message;
                    
                }
                context.Result = new InternalServerErrorObjectResult(json);
            }
            _logs.LogError(context.Exception.Message);
            //当删除操作过程中引发异常时,可使用 ExceptionHandled 属性指示该异常是否已在事件处理程序中得到处理。
            //如果将此属性设置为 true,则该异常被视为已处理,而不会在调用堆栈上进一步传递
            //反之该异常就会传递到调用堆栈上的下一个方法以进行处理
            context.ExceptionHandled = true;
        }

        private class InternalServerErrorObjectResult : ObjectResult
        {
            public InternalServerErrorObjectResult(object error) : base(error)
            {
                StatusCode = StatusCodes.Status500InternalServerError;
            }
        }

        private class JsonErrorResponse
        {
            public string Message { get; set; }

            public string DeveloperMessage { get; set; }
        }
    }

    
}
