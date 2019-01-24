﻿using IdentityServer4.Models;
using IdentityServer4.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Fate.Identity.Services
{
    public class ProfileService : IProfileService
    {
        public Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var subject = context.Subject ?? throw new ArgumentNullException(nameof(context.Subject));
            //context.IssuedClaims = subject.Claims.ToList();
            var claims = subject.Claims.ToList();
            claims.Add(new Claim(ClaimTypes.Name,claims.FirstOrDefault(c=>c.Type=="name")?.Value));
            context.IssuedClaims = claims;
            context.AddRequestedClaims(claims);
            return Task.CompletedTask;
        }

        public Task IsActiveAsync(IsActiveContext context)
        {
            var subject = context.Subject ?? throw new ArgumentNullException(nameof(context.Subject));
            context.IsActive = Convert.ToInt32(subject.Claims.FirstOrDefault(c=>c.Type=="sub").Value)>0;
            return Task.CompletedTask;
        }
    }
}
