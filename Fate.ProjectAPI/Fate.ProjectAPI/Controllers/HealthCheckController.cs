using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fate.Project.Infrastructure.RedisCache;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Fate.ProjectAPI.Controllers
{
    [Route("[controller]")]
    public class HealthCheckController : Controller
    {
        private readonly CsRedisHelper _csRedisHelper;

        public HealthCheckController(CsRedisHelper csRedisHelper)
        {
            _csRedisHelper = csRedisHelper;
        }

        [HttpGet("")]
        public IActionResult Get()
        {
            return Ok(RedisHelper.Get("NowTime"));
        }
    }
}