using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Fate.Project.Domain.AggregatesModel;

namespace Fate.ProjectAPI.Applications.Commands
{
    public class ProjectViewerCommand:IRequest<bool>
    {
        public int? ProjectId { get; set; }

        public int UserId { get; set; }

        public string UserName { get; set; }

        public string Avatar { get; set; }
    }
}
