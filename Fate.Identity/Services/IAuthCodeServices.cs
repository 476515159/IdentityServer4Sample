using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fate.Identity.Services
{
    public interface IAuthCodeServices
    {
        /// <summary>
        /// 验证手机号和验证码是否匹配
        /// </summary>
        /// <param name="Phone"></param>
        /// <param name="AuthCodes"></param>
        /// <returns></returns>
        bool Validate(string Phone,string AuthCodes);
    }
}
