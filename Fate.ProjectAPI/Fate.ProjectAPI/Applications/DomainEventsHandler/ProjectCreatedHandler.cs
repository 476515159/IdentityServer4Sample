using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fate.Project.Domain.Events;
using MediatR;
using DotNetCore.CAP;

namespace Fate.ProjectAPI.Applications.DomainEventsHandler
{
    public class ProjectCreatedHandler : INotificationHandler<ProjectCreatedDomainEvent>
    {
        private readonly ICapPublisher _capPublisher;
        public ProjectCreatedHandler(ICapPublisher capPublisher)
        {
            _capPublisher = capPublisher;
        }

        public async Task Handle(ProjectCreatedDomainEvent notification, CancellationToken cancellationToken)
        {
            await _capPublisher.PublishAsync("Fate.ProjectAPI.ProjectCreated", notification.Project);
        }
    }
}
