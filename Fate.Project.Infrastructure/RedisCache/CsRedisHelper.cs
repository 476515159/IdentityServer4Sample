using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fate.Project.Infrastructure.RedisCache
{
    public class CsRedisHelper
    {
        private readonly CsRedisOptions _options;
        public CsRedisHelper(IOptions<CsRedisOptions> options)
        {
            _options = options.Value;
            CSRedis.CSRedisClient csredis;
            if (_options.RedisConnectionStrings.Count == 1)
            {
                //普通模式
                csredis = new CSRedis.CSRedisClient(_options.RedisConnectionStrings[0]);
            }
            else
            {
                //集群模式
                //实现思路：根据key.GetHashCode() % 节点总数量，确定连向的节点
                //也可以自定义规则(第一个参数设置)
                csredis = new CSRedis.CSRedisClient(null, _options.RedisConnectionStrings.ToArray());
            }
            //初始化 RedisHelper
            RedisHelper.Initialization(csredis);
        }

        /// <summary>
        /// 添加缓存信息
        /// </summary>
        /// <param name="key">缓存的key</param>
        /// <param name="value">缓存的实体</param>
        /// <param name="ttl">过期时间</param>
        /// <param name="region">缓存所属分类，可以指定分类缓存过期</param>
        public void Add(string key, object value, TimeSpan ttl, string region)
        {
            key = GetKey(region, key);
            if (ttl.TotalMilliseconds <= 0)
            {
                return;
            }
            RedisHelper.Set(key, JsonConvert.SerializeObject(value), (int)ttl.TotalSeconds);
        }

        /// <summary>
        /// 批量移除regin开头的所有缓存记录
        /// </summary>
        /// <param name="region">缓存分类</param>
        public void ClearRegion(string region)
        {
            //获取所有满足条件的key
            var data = RedisHelper.Keys(_options.RedisKeyPrefix + "-" + region + "-*");
            //批量删除
            RedisHelper.Del(data);
        }

        /// <summary>
        /// 获取执行的缓存信息
        /// </summary>
        /// <param name="key">缓存key</param>
        /// <param name="region">缓存分类</param>
        /// <returns></returns>
        public T Get<T>(string key, string region)
        {
            key = GetKey(region, key);
            var result = RedisHelper.Get(key);
            if (!String.IsNullOrEmpty(result))
            {
                return JsonConvert.DeserializeObject<T>(result);
            }
            return default(T);
        }

        /// <summary>
        /// 获取格式化后的key
        /// </summary>
        /// <param name="region">分类标识</param>
        /// <param name="key">key</param>
        /// <returns></returns>
        private string GetKey(string region, string key)
        {
            return _options.RedisKeyPrefix + "-" + region + "-" + key;
        }
    }
}
