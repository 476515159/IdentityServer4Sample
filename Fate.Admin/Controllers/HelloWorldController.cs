using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Models;

namespace Fate.Admin.Controllers
{
    public class HelloWorldController : BaseController
    {
        public HelloWorldController(IOptionsMonitor<AppSetting> option) : base(option)
        {
        }

        public IActionResult Index()
        {
            var context = HttpContext.User.Identity.Name;
            var app = AppSetting;
            return View();
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignOutAsync("oidc");
            //var refererUrl = "http://localhost:8009" + "/connect/endsession";
            return View("index");
        }
    }
}