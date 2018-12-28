using System;
using System.Threading.Tasks;
using System.Net.Http;

namespace Resilience
{
    public interface IhttpClient
    {
        Task<HttpResponseMessage> PostAsync<T>(string url,T item,string authorizationToken=null,string requestId=null,string authorizationMethod="Bearer");

        Task<string> GetAsync(string url, string authorizationToken = null, string authorizationMethod = "Bearer");

        Task<HttpResponseMessage> PutAsync<T>(string url, T item, string authorizationToken = null, string requestId = null, string authorizationMethod = "Bearer");
    }
}
