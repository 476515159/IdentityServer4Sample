using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fate.ContactAPI.Data;
using Fate.ContactAPI.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Models;
using Models.Users;
using Microsoft.AspNetCore.Authorization;

namespace Fate.ContactAPI.Controllers
{
    [Route("api/contact")]
    [ApiController]
    [EnableCors("CorsTest")]
    public class ContactController : BaseController
    {
        private readonly IContactApplyRequestRepository _contactApplyRequest;
        private readonly IUserRepository _userRepository;
        private readonly IContactRepository _contactRepository;
        private readonly IUserService _userService;

        public ContactController(IContactApplyRequestRepository contactApplyRequest, IUserRepository userRepository, IContactRepository contactRepository, IUserService userService)
        {
            _contactApplyRequest = contactApplyRequest;
            _userRepository = userRepository;
            _contactRepository = contactRepository;
            _userService = userService;
        }

        //[HttpGet("")]
        //public IActionResult Get() => Ok(UserInfo);

        [HttpGet("apply-request")]
        public async Task<IActionResult> GetContactApplyRequest(CancellationToken cancellationToken)
        {
            return Ok(await _contactApplyRequest.GetContactApplyRequestAsync(UserInfo.UserId, cancellationToken));
        }

        /// <summary>
        /// 发起申请
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost("apply-request/{userId}")]
        public async Task<IActionResult> Add_ContactApplyRequestAsync(int userId, CancellationToken cancellationToken)
        {
            //获取当前登陆人的信息
            //var user = await _userRepository.GetUserInfoAsync(UserInfo.UserId, cancellationToken);
            //if (user == null)
            //{
            //    throw new OperationException("未找到该用户");
            //}
            bool result = await _contactApplyRequest.Add_ContactApplyRequestAsync(new Models.Users.ContactApplyRequest
            {
                ApplierId = UserInfo.UserId,
                UserId = userId,
                Avatar = UserInfo.Avatar,
                Company = UserInfo.Company,
                CreateTime = DateTime.Now,
                Name = UserInfo.Name,
                Title = UserInfo.Title,
                Approvaled=1
            }, cancellationToken);
            if (!result)
            {
                return BadRequest();
            }
            return Ok();
        }

        /// <summary>
        /// 处理申请
        /// </summary>
        /// <param name="ApplierId">申請人id</param>
        /// <param name="Approvaled">是否通过 0:未通过  1:待处理 2:已通过</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPut("apply-request/{ApplierId}&{Approvaled}")]
        public async Task<IActionResult> Approvaled_ContactApplyRequestAsync(int ApplierId, int Approvaled, CancellationToken cancellationToken)
        {
            bool result = await _contactApplyRequest.Approvaled_ContactApplyRequestAsync(new Models.Users.ContactApplyRequest
            {
                ApplierId = ApplierId,
                UserId = UserInfo.UserId,
                Approvaled = Approvaled,
                HandledTime = DateTime.Now
            }, cancellationToken);

            if (Approvaled == 2)
            {
                //调用UserApi接口获取信息
                var UserBaseInfo = await _userService.GetBaseInfoAsync(UserInfo.UserId);
                if (UserBaseInfo != null)
                {
                    await _contactRepository.Add_ContactAsync(ApplierId, UserBaseInfo, cancellationToken);
                }

                var ApprovyUserBaseInfo = await _userService.GetBaseInfoAsync(ApplierId);
                if (ApprovyUserBaseInfo != null)
                {
                    await _contactRepository.Add_ContactAsync(UserInfo.UserId, ApprovyUserBaseInfo, cancellationToken);
                }
            }
            

            if (!result)
            {
                return BadRequest();
            }
            return Ok();
        }
    }
}
