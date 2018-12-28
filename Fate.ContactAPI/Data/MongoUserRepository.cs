using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fate.ContactAPI.Data;
using Models.Dtos.Identity;
using MongoDB.Driver;
using Models.Users;

namespace Fate.ContactAPI.Services
{
    public class MongoUserRepository : IUserRepository
    {
        private readonly ContactContext _contactContext;

        public MongoUserRepository(ContactContext contactContext)
        {
            _contactContext = contactContext;
        }
        public async Task<UserIdentity> GetUserInfoAsync(int userId, CancellationToken cancellationToken)
        {
            var userInfo = await (await _contactContext.UserInfo.FindAsync(c => c.ID == userId,null,cancellationToken)).FirstOrDefaultAsync(cancellationToken);
            return new UserIdentity {
                UserId=userInfo.ID,
                Name=userInfo.Name,
                Avatar=userInfo.Avatar,
                Company=userInfo.Company,
                Title=userInfo.Title
            };
        }

        public async Task<bool> Update_UserInfo(UserIdentity model, CancellationToken cancellationToken)
        {
            try
            {
                //先查出当前更改人的好友
                var contactBook = await (await _contactContext.ContactBooks.FindAsync(c => c.UserId == model.UserId, null, cancellationToken))
                    .FirstOrDefaultAsync(cancellationToken);
                if (contactBook == null)
                    return true;
                //获取当前更改人的所有好友id
                var contacts = contactBook.Contacts.Select(c => c.UserId);
                //重新从通讯录中查询所有与更改人是好友的列表
                var filterDefinition = Builders<ContactBook>.Filter.And(
                        Builders<ContactBook>.Filter.In(c => c.UserId, contacts),//第一个条件 查询userid
                        Builders<ContactBook>.Filter.ElemMatch(c => c.Contacts, s => s.UserId == model.UserId)//第二个条件 只查出更改了信息的好友
                    );

                var lst = _contactContext.ContactBooks.Find(filterDefinition).ToList();

                #region mongoDb更新单个
                //var aa = Builders<ContactApplyRequest>.Update.Set(c => c.HandledTime, DateTime.Now);
                //_contactContext.ContactApplyRequests.UpdateOne(c => c.UserId == model.UserId, aa);
                #endregion

                //设置更新字段和值
                var updateDefinition = Builders<ContactBook>.Update
                    .Set("Contacts.$.Name", model.Name)
                    .Set("Contacts.$.Company", model.Company)
                    .Set("Contacts.$.Title", model.Title)
                    .Set("Contacts.$.Avatar", model.Avatar);

                var updateResult = _contactContext.ContactBooks.UpdateMany(filterDefinition, updateDefinition);
                return updateResult.MatchedCount == updateResult.ModifiedCount;
            }
            catch (Exception e)
            {
                return false;
            }
            
        }
    }
}
