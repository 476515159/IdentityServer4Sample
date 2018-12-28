using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Models.Users;
using System.Threading;
using MongoDB.Driver;

namespace Fate.ContactAPI.Data
{
    public class MongoContactApplyRequestRepository : IContactApplyRequestRepository
    {
        private readonly ContactContext _contactContext;

        public MongoContactApplyRequestRepository(ContactContext contactContext)
        {
            _contactContext = contactContext;
        }

        public async Task<bool> Add_ContactApplyRequestAsync(ContactApplyRequest model, CancellationToken cancellationToken)
        {
            try
            {
                var filter = Builders<ContactApplyRequest>.Filter.And(
                        Builders<ContactApplyRequest>.Filter.In(c=>c.ApplierId,new int[] { model.ApplierId,model.UserId}),
                        Builders<ContactApplyRequest>.Filter.In(c => c.UserId, new int[] { model.ApplierId, model.UserId }),
                        Builders<ContactApplyRequest>.Filter.Where(s=>s.Approvaled == 1)
                    );

                var applyRequest = await _contactContext.ContactApplyRequests.CountDocumentsAsync(filter,null,cancellationToken);
                if (applyRequest > 0)
                {
                    var update = Builders<ContactApplyRequest>.Update.Set(s => s.CreateTime, DateTime.Now);
                    var result = await _contactContext.ContactApplyRequests.UpdateOneAsync(filter, update, null, cancellationToken);
                    return result.ModifiedCount == result.MatchedCount;
                }
                await _contactContext.ContactApplyRequests.InsertOneAsync(model, null, cancellationToken);
                return true;
            }
            catch
            {
                return false;
            }


        }

        public async Task<bool> Approvaled_ContactApplyRequestAsync(ContactApplyRequest model, CancellationToken cancellationToken)
        {
            try
            {
                var updateDefinition = Builders<ContactApplyRequest>.Update
                    .Set(c => c.HandledTime, model.HandledTime)
                    .Set(c => c.Approvaled, model.Approvaled);
                var contactApply = await _contactContext.ContactApplyRequests.FindOneAndUpdateAsync(s => s.ApplierId == model.ApplierId && s.UserId == model.UserId, updateDefinition, null, cancellationToken);
                return contactApply != null;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<ContactApplyRequest>> GetContactApplyRequestAsync(int userId, CancellationToken cancellationToken)
        {
            return await (await _contactContext.ContactApplyRequests.FindAsync(a => a.UserId == userId)).ToListAsync(cancellationToken);
        }
    }
}
