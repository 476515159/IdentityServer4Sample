using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using IdentityServer4.Validation;
using IdentityServer4.Models;
using Fate.Identity.Services;
using System.Security.Claims;

namespace Fate.Identity.Authentication
{
    public class SmsAuthCodeValidator : IExtensionGrantValidator
    {

        private readonly IUserServices _userServices;
        private readonly IAuthCodeServices _authcodeServices;

        public SmsAuthCodeValidator(IUserServices userServices, IAuthCodeServices authcodeServices)
        {
            _userServices = userServices;
            _authcodeServices = authcodeServices;
        }

        public string GrantType => "sms_auth_code";

        public async Task ValidateAsync(ExtensionGrantValidationContext context)
        {
            string phone = context.Request.Raw["Phone"];
            string pwd = context.Request.Raw["PassWord"];
            string code = context.Request.Raw["AuthCode"];
            var errorValidationResult = new GrantValidationResult(TokenRequestErrors.InvalidGrant);

            if (string.IsNullOrWhiteSpace(phone) || string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(pwd))
            {
                context.Result = errorValidationResult;
                return;
            }

            Regex Reg_phone = new Regex(@"^((13[0-9])|(14[5|7])|(15([0-9]))|(17[0-9])|(18[0-9])|(19[0-9]))\d{8}$");
            if (!Reg_phone.IsMatch(phone))
            {
                context.Result = errorValidationResult;
                return;
            }

            if (!_authcodeServices.Validate(phone, code))
            {
                context.Result = errorValidationResult;
                return;
            }
            var userInfo = await _userServices.CreateOrCheck(new Models.Users.UserInfo { UserName = phone, UserPassword = pwd, Phone = phone });
            if (userInfo == null)
            {
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

            context.Result = new GrantValidationResult(userInfo.UserId.ToString(), GrantType,claims);
        }
    }
}
