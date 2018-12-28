using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Fate.Project.Domain.AggregatesModel;
using System.Threading;

namespace Fate.ProjectAPI.Applications.Commands
{
    public class ProjectViewerCommandHandler : IRequestHandler<ProjectViewerCommand,bool>
    {
        private readonly IProjectRepository _projectRepository;

        public ProjectViewerCommandHandler(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }

        public async Task<bool> Handle(ProjectViewerCommand request, CancellationToken cancellationToken)
        {
            var project = await _projectRepository.GetAsync(request.ProjectId.Value);
            if(project == null)
                throw new Fate.Project.Domain.Exceptions.ProjectDomainException($"未找到{request.ProjectId}项目");
            project.Add_ProjectViewer(request.UserId, request.UserName, request.Avatar);
            return await _projectRepository.UnitOfWork.SaveEntitiesAsync();

        }
    }
}
