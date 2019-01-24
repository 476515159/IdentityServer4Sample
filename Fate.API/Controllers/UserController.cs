using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fate.EfContext;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Models;
using Models.Users;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using DotNetCore.CAP;
using Models.Dtos.Identity;
using Microsoft.AspNetCore.Authorization;
using System.Net;

namespace Fate.API.Controllers
{
    //[Produces("application/json")]
    [Route("api/User"),Authorize]
    public class UserController : BaseController
    {
        private readonly AppSetting appSetting;
        private readonly EntityDbContext _db;
        private readonly ICapPublisher _capPublisher;
        public UserController([FromServices]EntityDbContext db, IOptionsMonitor<AppSetting> options, ICapPublisher capPublisher)
        {
            _db = db;
            appSetting = options.CurrentValue;
            _capPublisher = capPublisher;
        }

        private void RadisUpdate_UserInfoMessage(UserInfo model)
        {
            if (_db.Entry(model).Property(c => c.Name).IsModified ||
                _db.Entry(model).Property(c => c.Phone).IsModified ||
                _db.Entry(model).Property(c => c.Title).IsModified ||
                _db.Entry(model).Property(c => c.Avatar).IsModified ||
                _db.Entry(model).Property(c => c.Company).IsModified 
                )
            {
                _capPublisher.Publish("userapi.user_profile_changed", new UserIdentity {
                    Avatar=model.Avatar,
                    Company=model.Company,
                    Name=model.Name,
                    Title=model.Title,
                    UserId=model.ID
                });
            }
        }
        #region 测试RabbitMq
        //[HttpGet("test")]
        //[AllowAnonymous]
        //public IActionResult test()
        //{
        //    _capPublisher.Publish("test", new UserIdentity
        //    {
        //        Avatar = "test"

        //    });
        //    return Ok();
        //}


        //[CapSubscribe("test")]
        //[AllowAnonymous,NonAction]
        //public IActionResult testCap()
        //{
        //    return Ok("okCap");
        //}
        #endregion

        [HttpGet("Get")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public IActionResult Get()
        {

            var info = _db.UserInfo.AsNoTracking().FirstOrDefault(c => c.ID == UserInfo.UserId);
            return Json(info);
        }

        [HttpGet("GetBaseInfo/{userId}")]
        public IActionResult GetBaseInfo(int UserId)
        {

            var info = _db.UserInfo.AsNoTracking().FirstOrDefault(c => c.ID == UserId);
            if (info == null) return NotFound();
            return Json(info);
        }

        [HttpPost("InsertUsers")]
        public async Task<IActionResult> InsertUsers(UserInfo model)
        {
            var info = await _db.UserInfo.AddAsync(model);
            int count = await _db.SaveChangesAsync();
            return Json(count);
        }

        [HttpPatch("Update_User")]
        public async Task<IActionResult> Update_User([FromBody]JsonPatchDocument<UserInfo> model)
        {
            var user = await _db.UserInfo.FirstOrDefaultAsync(c => c.ID == UserInfo.UserId);
            if (user == null)
                throw new OperationException("用户不存在");

            model.ApplyTo(user);
            using (var trans = await _db.Database.BeginTransactionAsync())
            {
                //发布消息
                RadisUpdate_UserInfoMessage(user);

                //保存到数据库
                _db.UserInfo.Update(user);
                await _db.SaveChangesAsync();
                trans.Commit();
            }
                
            return Json(user);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(string values)
        {
            UserInfo model = Newtonsoft.Json.JsonConvert.DeserializeObject<UserInfo>(values);
            var userIdentity = new Models.Dtos.Identity.UserIdentity();
            var entity = await _db.UserInfo.FirstOrDefaultAsync(c => (c.Phone == model.Phone || c.UserName == model.UserName) && c.UserPassword==model.UserPassword);
            if (entity != null)
            {
                entity.LastLoginTime = DateTime.Now;
                _db.Update(entity);
                _db.SaveChanges();
            }
            return Json(entity);
        }

        [HttpPost, Route("CreateOrCheck")]
        public async Task<IActionResult> CreateOrCheck(string values)
        {
            try
            {
                //var content2 = Request;
                //var content = await Request.ReadFormAsync();
                //var a = content[""];
                UserInfo model = Newtonsoft.Json.JsonConvert.DeserializeObject<UserInfo>(values);
                var userIdentity = new Models.Dtos.Identity.UserIdentity();
                var entity = await _db.UserInfo.AsNoTracking().FirstOrDefaultAsync(c => c.Phone == model.Phone || c.UserName == model.UserName);
                if (entity == null)
                {
                    var result = await _db.UserInfo.AddAsync(model);
                    await _db.SaveChangesAsync();
                    userIdentity.UserId = result.Entity.ID;
                    userIdentity.Name = result.Entity.Name;
                    userIdentity.Title = result.Entity.Title;
                    userIdentity.Avatar = result.Entity.Avatar;
                    userIdentity.Company = result.Entity.Company;
                }
                userIdentity.UserId = entity.ID;
                userIdentity.Name = entity.Name;
                userIdentity.Title = entity.Title;
                userIdentity.Avatar = entity.Avatar;
                userIdentity.Company = entity.Company;
                return Json(userIdentity);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }


        /// <summary>
        /// 获取用户标签
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetUserTags")]
        public async Task<IActionResult> GetUserTags()
        {
            return Ok(await _db.UserTags.AsNoTracking().Where(c => c.UserId == UserInfo.UserId).ToListAsync());
        }

        /// <summary>
        /// 更新用户标签
        /// </summary>
        /// <returns></returns>
        [HttpPut, Route("updateTags")]
        //PUT请求：如果两个请求相同，后一个请求会把第一个请求覆盖掉。（所以PUT用来改资源）

        //Post请求：后一个请求不会把第一个请求覆盖掉。（所以Post用来增资源）
        public async Task<IActionResult> Update_UserTags([FromBody]List<string> tags)
        {
            try
            {
                var userTags = await _db.UserTags.Where(c => c.UserId == UserInfo.UserId).ToListAsync();
                var newTags = tags.Except(userTags.Select(s => s.Tag));
                await _db.UserTags.AddRangeAsync(newTags.Select(c => new UserTags { UserId = UserInfo.UserId, Tag = c, CreateTime = DateTime.Now }));
                await _db.SaveChangesAsync();
                return Ok("更新成功");
            }
            catch (Exception e)
            {
                return Json(e.Message);
            }

        }

        /// <summary>
        /// 搜索用户
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        [HttpGet("serch")]
        public async Task<IActionResult> Serch(string phone)
        {
            return Json(await _db.UserInfo.AsNoTracking().SingleOrDefaultAsync(c => c.Phone == phone));
        }
    }
}