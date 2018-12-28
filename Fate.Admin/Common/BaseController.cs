using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Models;
using Microsoft.AspNetCore.Authorization;

namespace Fate.Admin
{
    [Authorize]
    public abstract class BaseController : Controller
    {
        protected AppSetting AppSetting { get; set; }

        protected BaseController(IOptionsMonitor<AppSetting> option) => AppSetting = option.CurrentValue;
    }
}