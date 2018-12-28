using Fate.EfContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Models.Users;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using Microsoft.Extensions.Options;
using Models.Dtos;
using DnsClient;
using Resilience;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using IdentityModel.Client;

namespace Fate.Identity.Services
{
    public class UserServices : IUserServices
    {
        private readonly IhttpClient _client;
        private readonly string url;
        private readonly ILogger<UserServices> _logger;
        public UserServices(IhttpClient client, IOptions<ServiceDisvoveryOptions> serviceOptions, IDnsQuery dnsQuery, ILogger<UserServices> logger)
        {
            _client = client;
            _logger = logger;
            var address = dnsQuery.ResolveService("service.consul", serviceOptions.Value.ServiceName);
            var addressList = address.First()?.AddressList;
            var host = addressList.Any() ? addressList.First().ToString() : address.First().HostName;
            var port = address.First().Port;
            url = $"http://{host}:{port}";
        }
        /// <summary>
        /// 客戶端模式获取token
        /// </summary>
        /// <returns></returns>
        private async Task<string> GetToken()
        {
            var ids4 = "http://localhost:8009";
            //必须使用HTTPS这个问题，很多人都是部署到生产环境才发生的，因为生产环境很多情况下不会用 
            //localhost作为 IdentityServer4（后文简称 Ids4） 的地址，这个问题并不是 Ids4 引起的，
            //而是我们使用的 IdentityModel 这个组件引起的，它默认限制了当 Ids4 非 localhost 地址时，必须启用HTTPS。
            var discoveryClient = new DiscoveryClient(ids4) { Policy = { RequireHttps = false } };
            var diso = await discoveryClient.GetAsync();
            if (diso.IsError)
                throw new Exception(diso.Error);
            var tokenClient = new TokenClient(diso.TokenEndpoint, "client", "secret");
            //var tokenClient = new TokenClient("http://localhost:8009" + "/connect/token", "client", "secret");
            var tokenResponse = await tokenClient.RequestClientCredentialsAsync("Fate_UserAPI");
            if (tokenResponse.IsError)
                throw new Exception(tokenResponse.Error);
            return tokenResponse.AccessToken;
        }

        public async Task<Models.Dtos.Identity.UserIdentity> CreateOrCheck(UserInfo model)
        {
            try
            {
                //var formvalues = new Dictionary<string, string>();
                //formvalues.Add("", Newtonsoft.Json.JsonConvert.SerializeObject(model));
                //var content = new FormUrlEncodedContent(formvalues);
                try
                {
                    var result = await _client.PostAsync(url + "/api/User/CreateOrCheck", model,await GetToken());
                    //确保HTTP成功状态值  
                    result.EnsureSuccessStatusCode();
                    if (result.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var userInfoString = await result.Content.ReadAsStringAsync();
                        var userInfo = JsonConvert.DeserializeObject<Models.Dtos.Identity.UserIdentity>(userInfoString);
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

        public async Task<UserInfo> Login(UserInfo model)
        {
            try
            {
                try
                {
                    var result = await _client.PostAsync(url + "/api/User/login", model, await GetToken());
                    //确保HTTP成功状态值  
                    result.EnsureSuccessStatusCode();
                    if (result.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var userInfoString = await result.Content.ReadAsStringAsync();
                        if (string.IsNullOrWhiteSpace(userInfoString))
                            return null;
                        var userInfo = JsonConvert.DeserializeObject<UserInfo>(userInfoString);
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
