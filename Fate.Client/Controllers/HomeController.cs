using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Fate.Client.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Fate.Client.HttpClientApi;

namespace Fate.Client.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly HttpClientService _httpClientService;

        public HomeController(HttpClientService httpClientService)
        {
            _httpClientService = httpClientService;
        }

        public IActionResult Index()
        {
            return View();
        }
        [AllowAnonymous]
        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]//防止跨站请求伪造
        public async Task<IActionResult> Logout()
        {
            return new SignOutResult(new[]
            {
                CookieAuthenticationDefaults.AuthenticationScheme,
                "oidc"
            });

            //string response = await _httpClientService.PostAsync();

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignOutAsync("oidc");
            //var refererUrl = "http://localhost:8009"+ "/connect/endsession";
            //return Redirect(refererUrl);
            //return View("index");
        }
    }
}
