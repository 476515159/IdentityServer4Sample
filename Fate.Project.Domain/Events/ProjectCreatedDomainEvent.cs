using System;
using System.Collections.Generic;
using System.Text;
using Fate.Project.Domain.AggregatesModel;
using MediatR;

namespace Fate.Project.Domain.Events
{
    public class ProjectCreatedDomainEvent:INotification
    {
        public AggregatesModel.Project Project { get; set; }
    }
}
