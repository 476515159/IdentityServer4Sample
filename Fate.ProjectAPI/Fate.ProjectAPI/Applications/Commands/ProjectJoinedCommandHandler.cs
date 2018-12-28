using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Fate.Project.Domain.AggregatesModel;
using System.Threading;

namespace Fate.ProjectAPI.Applications.Commands
{
    public class ProjectJoinedCommandHandler : IRequestHandler<ProjectJoinedCommand, bool>
    {
        private readonly IProjectRepository _projectRepository;

        public ProjectJoinedCommandHandler(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }
        public async Task<bool> Handle(ProjectJoinedCommand request, CancellationToken cancellationToken)
        {
            var project = await _projectRepository.GetAsync(request.ProjectContributor.ProjectId.Value);
            if (project == null)
                throw new Fate.Project.Domain.Exceptions.ProjectDomainException($"未找到{request.ProjectContributor.ProjectId}项目");
            project.Add_ProjectContributor(request.ProjectContributor);
            return await _projectRepository.UnitOfWork.SaveEntitiesAsync();
        }
    }
}
