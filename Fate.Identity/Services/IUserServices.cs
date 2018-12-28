using Models.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fate.Identity.Services
{
    public interface IUserServices
    {
        /// <summary>
        /// 验证手机号是否被注册
        /// </summary>
        /// <param name="Phone"></param>
        /// <returns></returns>
        Task<Models.Dtos.Identity.UserIdentity> CreateOrCheck(UserInfo model);

        Task<UserInfo> Login(UserInfo model);
    }
}
