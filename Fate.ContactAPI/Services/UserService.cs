using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DnsClient;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Models.Dtos;
using Models.Users;
using Newtonsoft.Json;
using Resilience;

namespace Fate.ContactAPI.Services
{
    public class UserService : IUserService
    {
        private readonly IhttpClient _client;
        private readonly string url;
        private readonly ILogger<UserService> _logger;
        public UserService(IhttpClient client, IOptionsSnapshot<ServiceDisvoveryOptions> serviceOptions, IDnsQuery dnsQuery, ILogger<UserService> logger)
        {
            _client = client;
            _logger = logger;
            var address = dnsQuery.ResolveService("service.consul", serviceOptions.Value.ServiceName);
            var addressList = address.First()?.AddressList;
            var host = addressList.Any() ? addressList.First().ToString() : address.First().HostName;
            var port = address.First().Port;
            url = $"http://{host}:{port}";
        }

        public async Task<UserInfo> GetBaseInfoAsync(int UserId)
        {
            try
            {
                try
                {
                    var result = await _client.GetAsync(url + "/api/User/GetBaseInfo/"+UserId);
                    if (!string.IsNullOrWhiteSpace(result))
                    {
                        var userInfo = JsonConvert.DeserializeObject<UserInfo>(result);
                        return userInfo;
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError("CreateOrCheck在重试之后失败");
                    throw e;
                }
                return null;
            }
            catch
            {
                return null;
            }

        }
    }
}
