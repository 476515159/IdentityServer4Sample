using DotNetCore.CAP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Fate.Project.Domain.Events;
using System.Threading;

namespace Fate.ProjectAPI.Applications.DomainEventsHandler
{
    public class ProjectJoinedHandler:INotificationHandler<ProjectJoinedDomainEvent>
    {
        private readonly ICapPublisher _capPublisher;
        public ProjectJoinedHandler(ICapPublisher capPublisher)
        {
            _capPublisher = capPublisher;
        }

        public async Task Handle(ProjectJoinedDomainEvent notification, CancellationToken cancellationToken)
        {
            await _capPublisher.PublishAsync("Fate.ProjectAPI.ProjectJoined", notification);
        }
    }
}
