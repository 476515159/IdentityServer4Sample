using Models.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Fate.ContactAPI.Data
{
    public interface IContactApplyRequestRepository
    {
        /// <summary>
        /// 获取好友申请
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<List<ContactApplyRequest>> GetContactApplyRequestAsync(int userId, CancellationToken cancellationToken);

        /// <summary>
        /// 发起好友申请
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<bool> Add_ContactApplyRequestAsync(ContactApplyRequest model, CancellationToken cancellationToken);


        /// <summary>
        /// 处理好友申请
        /// </summary>
        /// <returns></returns>
        Task<bool> Approvaled_ContactApplyRequestAsync(ContactApplyRequest model, CancellationToken cancellationToken);
    }
}
