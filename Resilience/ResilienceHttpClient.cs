using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Wrap;
using zipkin4net.Tracers;
using zipkin4net.Transport.Http;

namespace Resilience
{
    public class ResilienceHttpClient : IhttpClient
    {
        private readonly HttpClient _httpClient;
        //根据url origin去创建policy
        private readonly Func<string, IEnumerable<Policy>> _policyCreator;

        //将policy打包组合成policy wraper,进行本地缓存
        private readonly ConcurrentDictionary<string, PolicyWrap> _policyWrappers;

        private readonly ILogger<ResilienceHttpClient> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ResilienceHttpClient(string applicationName,Func<string, IEnumerable<Policy>> policyCreator, 
            ILogger<ResilienceHttpClient> logger, 
            IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = new HttpClient(new TracingHandler(applicationName));//和startup一样
            _policyCreator = policyCreator;
            _policyWrappers = new ConcurrentDictionary<string, PolicyWrap>();
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public Task<string> GetAsync(string url, string authorizationToken = null, string authorizationMethod = "Bearer")
        {
            var origin = NormalizeOrigin(GetOriginFromuri(url));
            return HttpInvoker(origin, async x => {
                var requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
                if (authorizationToken != null)
                {
                    requestMessage.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(authorizationMethod, authorizationToken);
                }

                var response = await _httpClient.SendAsync(requestMessage);
                if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    throw new HttpRequestException();
                }
                response.EnsureSuccessStatusCode();
                if (!response.IsSuccessStatusCode) return null;
                return await response.Content.ReadAsStringAsync();
            });
        }

        public async Task<HttpResponseMessage> PostAsync<T>(string url, T item, string authorizationToken = null, string requestId = null, string authorizationMethod = "Bearer")
        {
            return await DoPostAsync(HttpMethod.Post, url, item, authorizationToken, requestId, authorizationMethod);
        }

        public async Task<HttpResponseMessage> PutAsync<T>(string url, T item, string authorizationToken = null, string requestId = null, string authorizationMethod = "Bearer")
        {
            return await DoPostAsync(HttpMethod.Put, url, item, authorizationToken, requestId, authorizationMethod);
        }

        private Task<HttpResponseMessage> DoPostAsync<T>(HttpMethod method,string url, T item, string authorizationToken = null, string requestId = null, string authorizationMethod = "Bearer")
        {
            if (method != HttpMethod.Post && method != HttpMethod.Put)
            {
                throw new ArgumentException("方法不正确",nameof(method));
            }

            var origin = NormalizeOrigin(GetOriginFromuri(url));
            return HttpInvoker(origin,async x=> {
                var requestMessage = new HttpRequestMessage(method, url);
                //SetAuthorizationHeader(requestMessage);
                var formvalues = new Dictionary<string, string>();
                formvalues.Add("", Newtonsoft.Json.JsonConvert.SerializeObject(item));
                requestMessage.Content = new FormUrlEncodedContent(formvalues);
                //requestMessage.Content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(item), Encoding.UTF8, "application/json");

                if (authorizationToken != null)
                {
                    requestMessage.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(authorizationMethod, authorizationToken);
                }

                if (requestId != null)
                {
                    requestMessage.Headers.Add("x-requestid", requestId);
                }

                var response = await _httpClient.SendAsync(requestMessage);
                if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    throw new HttpRequestException();
                }

                return response;
            });
        }

        private async Task<T> HttpInvoker<T>(string normalizeOrigin, Func<Context,Task<T>> action)
        {
            //var normalizeOrigin = NormalizeOrigin(origin);
            if (!_policyWrappers.TryGetValue(normalizeOrigin, out PolicyWrap policyWrap))
            {
                policyWrap = Policy.Wrap(_policyCreator(normalizeOrigin).ToArray());
                _policyWrappers.TryAdd(normalizeOrigin, policyWrap);
            }
            return await policyWrap.ExecuteAsync(action, new Context(normalizeOrigin));
        }

        private static string NormalizeOrigin(string origin)
        {
            return origin?.Trim()?.ToLower();
        }

        private static string GetOriginFromuri(string uri)
        {
            var url = new Uri(uri);
            var origin = $"{url.Scheme}://{url.DnsSafeHost}:{url.Port}";
            return origin;

        }

        private void SetAuthorizationHeader(HttpRequestMessage requestMessage)
        {
            string authorizationHeader = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];
            if (!string.IsNullOrWhiteSpace(authorizationHeader))
            {
                requestMessage.Headers.Add("Authorization", authorizationHeader);
            }
        }
    }
}
