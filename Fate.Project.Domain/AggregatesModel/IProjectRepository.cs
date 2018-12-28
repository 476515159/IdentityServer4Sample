using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Fate.Project.Domain.AggregatesModel
{
    public interface IProjectRepository:IRepository<Project>
    {
        Task<Project> GetAsync(int Id);

        Task<Project> AddAsync(Project model);

        Project Update(Project model);
    }
}
