using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DotNetCore.CAP;
using Fate.ContactAPI.IntegrationEvents.Events;
using Fate.ContactAPI.Services;
using Models.Dtos.Identity;

namespace Fate.ContactAPI.IntegrationEvents.EventHanding
{
    public class UserProfileChangedEventHander:ICapSubscribe
    {
        private readonly IUserRepository _userRepository;

        public UserProfileChangedEventHander(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [CapSubscribe("userapi.user_profile_changed")]
        public async Task Update_ContactInfo(UserIdentity model)
        {
            await _userRepository.Update_UserInfo(model, default(CancellationToken));
        }
    }
}
