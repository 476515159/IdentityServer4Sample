using System;
using System.Collections.Generic;
using System.Text;

namespace Fate.Project.Infrastructure.RedisCache
{
    public interface ICsRedisHelper
    {
        void Add<T>(string key, T value, TimeSpan ttl, string region);

        void AddAndDelete<T>(string key, T value, TimeSpan ttl, string region);
    }
}
