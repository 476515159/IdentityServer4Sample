using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fate.Identity.Services;
using Fate.Identity.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using IdentityServer4.Services;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using IdentityServer4;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using IdentityServer4.Events;
using IdentityServer4.Extensions;

namespace Fate.Identity.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserServices _userServices;
        private readonly IIdentityServerInteractionService _identityServerInteractionService;
        private readonly IClientStore _clientStore;
        private readonly IEventService _events;

        public AccountController(IUserServices userServices, IIdentityServerInteractionService identityServerInteractionService, IClientStore clientStore, IEventService events)
        {
            _userServices = userServices;
            _identityServerInteractionService = identityServerInteractionService;
            _clientStore = clientStore;
            _events = events;
        }

        private async Task<LoginViewModel> CreateViewModel(string returnUrl)
        {
            LoginViewModel loginViewModel = new LoginViewModel
            {
                ReturnUrl = returnUrl,
            };
            var request = await _identityServerInteractionService.GetAuthorizationContextAsync(returnUrl);
            if (request == null)
                return loginViewModel;



            if (!string.IsNullOrWhiteSpace(request.ClientId))
            {
                var client = await _clientStore.FindEnabledClientByIdAsync(request.ClientId);
                if (client != null)
                {
                    loginViewModel.ClientName = client.ClientName;
                    loginViewModel.ClientUrl = client.ClientUri;
                    loginViewModel.ClientLogoUrl = client.LogoUri;
                    if (client.IdentityProviderRestrictions != null && client.IdentityProviderRestrictions.Any())
                    {
                        //providers = providers.Where(provider => client.IdentityProviderRestrictions.Contains(provider.AuthenticationScheme)).ToList();
                    }
                }
            }
            return loginViewModel;


        }

        [HttpGet]
        public async Task<IActionResult> Login(string returnUrl)
        {
            LoginViewModel loginViewModel = await CreateViewModel(returnUrl);
            return View(loginViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            #region 基础验证
            if (string.IsNullOrEmpty(model.UserName))
            {
                ModelState.AddModelError("", "请输入用户名");
                return View(model);
            }
            if (string.IsNullOrEmpty(model.Password))
            {
                ModelState.AddModelError("", "请输入密码");
                return View(model);
            }
            #endregion

            var userInfo = await _userServices.Login(new Models.Users.UserInfo { UserName = model.UserName, UserPassword = model.Password });
            if (userInfo == null)
            {
                ModelState.AddModelError("", "用户名或密码错误");
                return View(model);
            }


            AuthenticationProperties authenticationProperties = null;
            if (model.RememberLogin)
            {
                //保存30分钟
                authenticationProperties = new AuthenticationProperties { IsPersistent = true, ExpiresUtc = DateTimeOffset.UtcNow.AddMilliseconds(30) };

            }
            Claim[] claims = new Claim[]
            {
                new Claim("title",userInfo.Title??string.Empty),
                new Claim("company",userInfo.Company??string.Empty),
                new Claim("avatar",userInfo.Avatar??string.Empty),
            };
            await HttpContext.SignInAsync(userInfo.ID.ToString(), userInfo.UserName, authenticationProperties
                , claims);
            if (_identityServerInteractionService.IsValidReturnUrl(model.ReturnUrl) || Url.IsLocalUrl(model.ReturnUrl))
            {
                return Redirect(model.ReturnUrl);
            }
            return Redirect("~/");
        }

        #region 错误页面

        public async Task<IActionResult> Error(string errorId)
        {
            var model = await _identityServerInteractionService.GetErrorContextAsync(errorId);

            return View(model);
        }

        #endregion


        #region 退出
        [HttpGet]
        public async Task<IActionResult> Logout(string logoutId)
        {
            await HttpContext.SignOutAsync();
            if (string.IsNullOrEmpty(logoutId))
            {
                var refererUrl = Request.Headers["Referer"].ToString();
                return Redirect(refererUrl);
            }

            var context = await _identityServerInteractionService.GetLogoutContextAsync(logoutId);
            LogoutViewModel model = new LogoutViewModel
            {
                RedirectUri = new Uri(context.PostLogoutRedirectUri).GetLeftPart(UriPartial.Authority),
                SignOutIframeUrl = context.SignOutIFrameUrl,
                logoutId = logoutId,
                ShowSignoutPrompt = false
            };
            
            if (context?.ShowSignoutPrompt == false && false)//如果显示提示界面,进入确认页面目前无法返回之前的地址
            {
                return await Logout(model);
            }
            model.ShowSignoutPrompt = true;
            return View("Loggedout", model);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout(LogoutViewModel viewModel)
        {
            var context = await _identityServerInteractionService.GetLogoutContextAsync(viewModel.logoutId);
            //await HttpContext.SignOutAsync(IdentityServerConstants.DefaultCookieAuthenticationScheme);第一种注销

            //第二种注销
            var user = HttpContext.User;
            if (user?.Identity.IsAuthenticated == true)
            {
                //删除本地授权Cookies
                await HttpContext.SignOutAsync();
                await _events.RaiseAsync(new UserLogoutSuccessEvent(user.GetSubjectId(), user.GetDisplayName()));
            }

            if (context != null)
            {
                if (!string.IsNullOrWhiteSpace(context.PostLogoutRedirectUri))
                    return Redirect(context.PostLogoutRedirectUri);

                if (viewModel.ShowSignoutPrompt)
                {
                    //如果进入确认页面
                    var client = await _clientStore.FindClientByIdAsync(context.ClientId);
                    return Redirect(client?.PostLogoutRedirectUris.FirstOrDefault() ?? "~/");
                }
            }
            var refererUrl = Request.Headers["Referer"].ToString();
            return Redirect(refererUrl);
        }

        #endregion

    }
}