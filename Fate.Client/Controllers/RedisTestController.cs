using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Distributed;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace Fate.Client.Controllers
{
    [AllowAnonymous]
    [Route("[controller]")]
    public class RedisTestController : Controller
    {
        private readonly IDistributedCache _distributedCache;
        const string key = "test_key";
        public RedisTestController(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }
        [HttpGet("Index")]
        public async Task<IActionResult> Index()
        {
            HttpContext.Session.SetString("user", "lijinlong");
            var testSesson = HttpContext.Session.GetString("user");
            var valueByte = await _distributedCache.GetAsync(key);
            if (valueByte == null)
            {
                //_distributedCache.SetAsync("object",));
                await _distributedCache.SetStringAsync(key, DateTime.Now.ToLongTimeString(), new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10) });
                string value = await _distributedCache.GetStringAsync(key);
                //_distributedCache.Refresh
                Console.WriteLine(value);
            }
            else
            {
                var valueString = Encoding.UTF8.GetString(valueByte);
                Console.WriteLine(valueString);
            }
            return Ok();
        }
        [HttpGet("Get")]
        public async Task<IActionResult> Get()
        {
            string value = await _distributedCache.GetStringAsync(key);
            return Ok(value);
        }
    }
}