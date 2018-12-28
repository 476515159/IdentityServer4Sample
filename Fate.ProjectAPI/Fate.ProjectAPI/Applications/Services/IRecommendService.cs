using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fate.ProjectAPI.Applications.Services
{
    public interface IRecommendService
    {
        /// <summary>
        /// 是否有查看项目的权利
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<bool> IsProjectRecommend(int projectId,int userId);
    }
}
