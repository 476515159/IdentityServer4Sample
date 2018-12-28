using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Fate.API.Controllers
{
    [Route("[controller]")]
    public class HealthCheckController : Controller
    {
        [HttpGet("")]
        public IActionResult Get()
        {
            return Ok();
        }
    }
}