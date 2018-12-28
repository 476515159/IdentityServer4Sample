using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fate.Project.Domain.AggregatesModel;
using MediatR;

namespace Fate.ProjectAPI.Applications.Commands
{
    public class ProjectCreatedCommandHandler : IRequestHandler<ProjectCreatedCommand, Fate.Project.Domain.AggregatesModel.Project>
    {
        private readonly IProjectRepository _projectRepository;

        public ProjectCreatedCommandHandler(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }
        public async Task<Project.Domain.AggregatesModel.Project> Handle(ProjectCreatedCommand request, CancellationToken cancellationToken)
        {
            var projectResult = await _projectRepository.AddAsync(request.Project);
            await _projectRepository.UnitOfWork.SaveEntitiesAsync();
            return projectResult;
        }
    }
}
