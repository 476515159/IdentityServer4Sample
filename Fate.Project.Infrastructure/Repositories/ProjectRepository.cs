using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Fate.Project.Domain;
using Fate.Project.Domain.AggregatesModel;
using ProjectEntity = Fate.Project.Domain.AggregatesModel.Project;
using Microsoft.EntityFrameworkCore;

namespace Fate.Project.Infrastructure.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly ProjectContext _projectContext;

        public ProjectRepository(ProjectContext projectContext)
        {
            _projectContext = projectContext;
        }

        public IUnitOfWork UnitOfWork => _projectContext;

        public async Task<ProjectEntity> AddAsync(ProjectEntity model)
        {
            try
            {
                if (model.IsTransient())
                {
                    var result = await _projectContext.AddAsync(model);
                    return result.Entity;
                }
                else
                    return model;
            }
            catch
            {
                return null;
            }
        }

        public async Task<ProjectEntity> GetAsync(int Id)
        {
            return await _projectContext.Projects.AsNoTracking()
                .Include(c => c.Viewers).AsNoTracking()
                .Include(c => c.Properties).AsNoTracking()
                .Include(c=>c.VisibleRule).AsNoTracking()
                .Include(c=>c.Contributors).AsNoTracking()
                .SingleOrDefaultAsync(c=>c.Id==Id);
        }

        public ProjectEntity Update(ProjectEntity model)
        {
            try
            {
                var result = _projectContext.Update(model);
                return result.Entity;
            }
            catch
            {
                return null;
            }
        }
    }
}
