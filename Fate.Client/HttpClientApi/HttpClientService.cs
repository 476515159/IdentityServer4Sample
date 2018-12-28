using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Fate.Client.HttpClientApi
{
    public class HttpClientService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HttpClientService(IHttpClientFactory httpclientfactory, IHttpContextAccessor httpcontextaccessor)
        {
            _httpClientFactory = httpclientfactory;
            _httpContextAccessor = httpcontextaccessor;
        }

        public async Task<string> PostAsync()
        {
            HttpClient client = _httpClientFactory.CreateClient("identity");
            string token = await _httpContextAccessor.HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);
            //client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("","");
            client.SetBearerToken(token);
            string response = await (await client.PostAsync("/Account/Logout",null)).Content.ReadAsStringAsync();
            return response;
        }
    }
}
