using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Models.Users;

namespace Fate.ContactAPI.Data
{
    public interface IContactRepository
    {
        Task<bool> Add_ContactAsync(int userId, UserInfo contact, CancellationToken cancellationToken);
    }
}
