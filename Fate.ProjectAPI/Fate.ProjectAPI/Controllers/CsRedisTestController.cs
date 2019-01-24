using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fate.Project.Infrastructure.RedisCache;
using Microsoft.AspNetCore.Mvc;

namespace Fate.ProjectAPI.Controllers
{
    [Route("[controller]")]
    public class CsRedisTestController : Controller
    {
        private readonly CsRedisHelper _csRedisHelper;
        string key = "NowTime";
        public CsRedisTestController(CsRedisHelper csRedisHelper)
        {
            _csRedisHelper = csRedisHelper;
        }
        [HttpGet("")]
        public IActionResult Index()
        {
            string datetime = DateTime.Now.ToLongTimeString();
            RedisHelper.Set(key, datetime);
            return Ok(datetime);
        }

        [HttpGet("Get")]
        public IActionResult Get()
        {
            string datetime = RedisHelper.Get(key);
            return Ok(datetime);
        }
    }
}