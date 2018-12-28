using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fate.Identity.Services
{
    public class AuthCodeServices : IAuthCodeServices
    {
        public bool Validate(string Phone, string AuthCodes)
        {
            return true;
        }
    }
}
