using Fate.Identity.Services;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Fate.Identity.Authentication
{
    public class ResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        private readonly IUserServices _userServices;

        public ResourceOwnerPasswordValidator(IUserServices userServices)
        {
            _userServices = userServices;
        }

        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            var errorValidationResult = new GrantValidationResult(TokenRequestErrors.InvalidClient);
            var userInfo = await _userServices.CreateOrCheck(new Models.Users.UserInfo { UserName = context.UserName, UserPassword = context.Password });
            errorValidationResult.Error = "test";
            if (userInfo == null)
            {
                errorValidationResult.Error = "用户名或密码错误";
                context.Result = errorValidationResult;
                return;
            }
            Claim[] claims = new Claim[]
            {
                new Claim("name",userInfo.Name??string.Empty),
                new Claim("title",userInfo.Title??string.Empty),
                new Claim("company",userInfo.Company??string.Empty),
                new Claim("avatar",userInfo.Avatar??string.Empty),
            };
            context.Result = new GrantValidationResult(userInfo.UserId.ToString(), "password", claims);
        }
    }
}
