using System;
using System.Collections.Generic;
using System.Text;
using Fate.Project.Domain.AggregatesModel;
using MediatR;

namespace Fate.Project.Domain.Events
{
    public class ProjectJoinedDomainEvent : INotification
    {
        public string Name { get; set; }

        public string Avatar { get; set; }
        public ProjectContributor ProjectContributor { get; set; }
    }
}
