using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Models.Dtos.Identity;

namespace Fate.ProjectAPI
{
    public class BaseController : ControllerBase
    {
        protected UserIdentity UserIdentity
        {
            get
            {
                return new UserIdentity
                {
                    UserId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "sub").Value),
                    Avatar = User.Claims.FirstOrDefault(c => c.Type == "avatar").Value,
                    Company = User.Claims.FirstOrDefault(c => c.Type == "company").Value,
                    Name = User.Claims.FirstOrDefault(c => c.Type == "name").Value,
                    Title = User.Claims.FirstOrDefault(c => c.Type == "title").Value,
                };
            }
        }
    }
}