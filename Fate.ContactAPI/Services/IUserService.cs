using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Models.Users;

namespace Fate.ContactAPI.Services
{
    public interface IUserService
    {
        Task<UserInfo> GetBaseInfoAsync(int UserId);
    }
}
