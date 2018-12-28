using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fate.ProjectAPI.Applications.Queries
{
    public interface IProjectQueries
    {
        Task<List<dynamic>> Get_ProjectByUserId(int UserId);

        Task<dynamic> Get_ProjectDetail(int ProjectId);
    }
}
