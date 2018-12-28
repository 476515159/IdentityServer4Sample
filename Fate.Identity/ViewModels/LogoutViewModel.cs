using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fate.Identity.ViewModels
{
    public class LogoutViewModel
    {
        public string logoutId { get; set; }

        public bool ShowSignoutPrompt { get; set; }


        public string RedirectUri { get; set; }
        public string SignOutIframeUrl { get; set; }
    }
}
