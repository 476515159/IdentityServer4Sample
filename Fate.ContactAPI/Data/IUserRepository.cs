using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Models.Dtos.Identity;
using System.Threading;

namespace Fate.ContactAPI.Services
{
    public interface IUserRepository
    {
        Task<UserIdentity> GetUserInfoAsync(int userId,CancellationToken cancellationToken);

        /// <summary>
        /// 更新个人信息,同时更新通讯录好友所显示的信息
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> Update_UserInfo(UserIdentity model, CancellationToken cancellationToken);
    }
}
