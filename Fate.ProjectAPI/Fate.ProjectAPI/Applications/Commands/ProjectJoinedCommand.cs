using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Fate.Project.Domain.AggregatesModel;

namespace Fate.ProjectAPI.Applications.Commands
{
    public class ProjectJoinedCommand:IRequest<bool>
    {
        public ProjectContributor ProjectContributor { get; set; }
    }
}
