using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Models.Users;
using System.Threading;
using Models.Dtos.Identity;
using MongoDB.Driver;

namespace Fate.ContactAPI.Data
{
    public class MongoContactRepository : IContactRepository
    {
        private readonly ContactContext _contactContext;

        public MongoContactRepository(ContactContext contactContext)
        {
            _contactContext = contactContext;
        }

        public async Task<bool> Add_ContactAsync(int userId, UserInfo contact, CancellationToken cancellationToken)
        {
            try
            {
                var filter = Builders<ContactBook>.Filter.Eq(c => c.UserId, userId);
                var contactBook = await (await _contactContext.ContactBooks.FindAsync(filter, null, cancellationToken)).FirstOrDefaultAsync(cancellationToken);
                if (contactBook == null)
                {
                    await _contactContext.ContactBooks.InsertOneAsync(new ContactBook
                    {
                        UserId = userId

                    }, null, cancellationToken);
                }

                //设置更新字段和值
                var updateDefinition = Builders<ContactBook>.Update.AddToSet(c => c.Contacts, new Contact
                {
                    UserId = contact.ID,
                    Avatar = contact.Avatar,
                    Company = contact.Company,
                    Name = contact.Name,
                    Title = contact.Title
                });
                var result = await _contactContext.ContactBooks.UpdateOneAsync(filter, updateDefinition, null, cancellationToken);


                return result.MatchedCount == result.ModifiedCount && result.ModifiedCount == 1;
            }
            catch (Exception e)
            {
                return false;
            }

        }
    }
}
