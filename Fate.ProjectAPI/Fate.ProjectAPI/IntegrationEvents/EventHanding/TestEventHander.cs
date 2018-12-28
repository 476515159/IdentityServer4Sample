using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotNetCore.CAP;
using Fate.Project.Domain.Events;

namespace Fate.ProjectAPI.IntegrationEvents.EventHanding
{
    public class TestEventHander:ICapSubscribe
    {
        //[CapSubscribe("Fate.ProjectAPI.ProjectCreated")]
        //public async Task ProjectCreated(Project.Domain.AggregatesModel.Project @event)
        //{
        //    Console.Write(@event);
        //    //await _recommendContext.AddAsync(new RecommendProject { Avatar=@event.Avatar,Company=@event.Company,UserId=@event.UserId,ProjectId=@event.Id});
        //    //await _recommendContext.SaveChangesAsync();
        //}
    }
}
