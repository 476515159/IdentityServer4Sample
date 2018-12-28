using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Fate.Project.Domain.AggregatesModel;

namespace Fate.ProjectAPI.Applications.Commands
{
    public class ProjectCreatedCommand:IRequest<Project.Domain.AggregatesModel.Project>
    {
        public Project.Domain.AggregatesModel.Project Project { get; set; }
    }
}
