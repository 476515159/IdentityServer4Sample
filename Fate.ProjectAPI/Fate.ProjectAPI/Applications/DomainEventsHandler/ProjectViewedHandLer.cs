using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using DotNetCore.CAP;
using Fate.Project.Domain.Events;
using System.Threading;

namespace Fate.ProjectAPI.Applications.DomainEventsHandler
{
    public class ProjectViewedHandLer:INotificationHandler<ProjectViewerDomainEvent>
    {
        private readonly ICapPublisher _capPublisher;
        public ProjectViewedHandLer(ICapPublisher capPublisher)
        {
            _capPublisher = capPublisher;
        }

        public async Task Handle(ProjectViewerDomainEvent notification, CancellationToken cancellationToken)
        {
            await _capPublisher.PublishAsync("Fate.ProjectAPI.ProjectViewed", notification);
        }
    }
}
