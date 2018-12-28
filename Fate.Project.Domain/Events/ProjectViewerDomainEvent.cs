using System;
using System.Collections.Generic;
using System.Text;
using Fate.Project.Domain.AggregatesModel;
using MediatR;

namespace Fate.Project.Domain.Events
{
    public class ProjectViewerDomainEvent:INotification
    {

        public string Name { get; set; }

        public string Avatar { get; set; }
        public ProjectViewer ProjectViewer { get; set; }
        //public ProjectViewerDomainEvent(ProjectViewer projectViewer)
        //{
        //    ProjectViewer = projectViewer;
        //}

    }
}
