using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotNetCore.CAP;
using Fate.Recommend.Data;
using Fate.Recommend.IntegrationEvents.Events;
using Fate.Recommend.Model;

namespace Fate.Recommend.IntegrationEvents.EventHanding
{
    public class ProjectCreatedHander:ICapSubscribe
    {
        private readonly RecommendContext _recommendContext;

        public ProjectCreatedHander(RecommendContext recommendContext)
        {
            _recommendContext = recommendContext;
        }

        [CapSubscribe("Fate.ProjectAPI.ProjectCreated")]
        public async Task ProjectCreated(ProjectCreatedEvent @event)
        {
            await _recommendContext.AddAsync(new RecommendProject { Avatar = @event.Avatar, Company = @event.Company, UserId = @event.UserId, ProjectId = @event.Id });
            await _recommendContext.SaveChangesAsync();
        }
    }
}
