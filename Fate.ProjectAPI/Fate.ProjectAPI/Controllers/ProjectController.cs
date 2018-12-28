using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using ProjectEntity = Fate.Project.Domain.AggregatesModel.Project;
using Fate.ProjectAPI.Applications.Commands;
using Fate.Project.Domain.AggregatesModel;
using Fate.ProjectAPI.Applications.Services;
using Fate.ProjectAPI.Applications.Queries;
using Microsoft.AspNetCore.Authorization;
using DotNetCore.CAP;
using Fate.Project.Domain.Events;

namespace Fate.ProjectAPI.Controllers
{
    [Route("api/project")]
    [ApiController]
    public class ProjectController : BaseController
    {
        private readonly IMediator _mediator;
        private readonly IRecommendService _recommendService;
        private readonly IProjectQueries _projectQueries;

        public ProjectController(IMediator mediator, IRecommendService recommendService, IProjectQueries projectQueries)
        {
            _mediator = mediator;
            _recommendService = recommendService;
            _projectQueries = projectQueries;
        }

        //[CapSubscribe("Fate.ProjectAPI.ProjectCreated")]
        //public IActionResult test(ProjectEntity @event)
        //{
        //    return Ok(@event);
        //}

        [HttpGet("")]
        public async Task<IActionResult> Get_ProjectByUserId()
        {
            var result = await _projectQueries.Get_ProjectByUserId(UserIdentity.UserId);
            return Ok(result);
        }

        [HttpGet("my/{projectId}")]
        public async Task<IActionResult> Get_ProjectDetailByProjectId(int projectId)
        {
            if (await _recommendService.IsProjectRecommend(projectId, UserIdentity.UserId))
            {
                return BadRequest("没有查看该项目的权限");
            }
            var result = await _projectQueries.Get_ProjectDetail(projectId);
            return Ok(result);
        }

        [HttpPost("add_project")]
        public async Task<IActionResult> Add_Project([FromBody]ProjectEntity model)
        {
            model.UserId = UserIdentity.UserId;
            var result = await _mediator.Send(new ProjectCreatedCommand { Project = model });
            return Ok(result);
        }

        [HttpPost("add_projectjoiner")]
        public async Task<IActionResult> Add_ProjectJoiner([FromBody]ProjectContributor model)
        {
            if (await _recommendService.IsProjectRecommend(model.ProjectId.Value, UserIdentity.UserId))
            {
                return BadRequest("没有查看该项目的权限");
            }
            model.UserId = UserIdentity.UserId;
            model.UserName = UserIdentity.Name;
            model.Avatar = UserIdentity.Avatar;
            var result = await _mediator.Send(new ProjectJoinedCommand { ProjectContributor = model });
            return Ok(result);
        }

        [HttpPost("add_projectviewer/{projectid}")]
        public async Task<IActionResult> Add_ProjectViewer(int ProjectId)
        {
            if (await _recommendService.IsProjectRecommend(ProjectId, UserIdentity.UserId))
            {
                return BadRequest("没有查看该项目的权限");
            }
            var result = await _mediator.Send(new ProjectViewerCommand { UserId = UserIdentity.UserId,ProjectId=ProjectId,Avatar=UserIdentity.Avatar,UserName=UserIdentity.Name });
            return Ok(result);
        }
    }
}
