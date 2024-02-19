using FreeRedis;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic;
using TencentCloud.Mna.V20210119.Models;
using Volo.Abp.Application.Services;
using Volo.Abp.Caching;
using Volo.Abp.DependencyInjection;
using Yi.Framework.Rbac.Application.Contracts.Dtos.MonitorCache;
using Yi.Framework.Rbac.Application.Contracts.IServices;

namespace Yi.Framework.Rbac.Application.Services.Monitor
{
    public class MonitorCacheService : ApplicationService, IMonitorCacheService
    {
        public IAbpLazyServiceProvider LazyServiceProvider { get; set; }

        /// <summary>
        /// 缓存前缀
        /// </summary>
        private string CacheKeyPrefix => LazyServiceProvider.LazyGetRequiredService<IOptions<AbpDistributedCacheOptions>>().Value.KeyPrefix;

        private bool EnableRedisCache
        {
            get
            {
                var redisEnabled = LazyServiceProvider.LazyGetRequiredService<IConfiguration>()["Redis:IsEnabled"];
                return redisEnabled.IsNullOrEmpty() || bool.Parse(redisEnabled);
            }
        }
        /// <summary>
        /// 使用懒加载防止报错
        /// </summary>
        private IRedisClient RedisClient => LazyServiceProvider.LazyGetRequiredService<IRedisClient>();

        /// <summary>
        /// 获取所有key并分组
        /// </summary>
        /// <returns></returns>
        [HttpGet("monitor-cache/name")]
        public List<MonitorCacheNameGetListOutputDto> GetName()
        {
            VerifyRedisCacheEnable();
            var keys = RedisClient.Keys(CacheKeyPrefix + "*");
            var result = GroupedKeys(keys.ToList());
            var output = result.Select(x => new MonitorCacheNameGetListOutputDto { CacheName = x }).ToList();
            return output;
        }

        private List<string> GroupedKeys(List<string> keys)
        {
            HashSet<string> resultSet = new HashSet<string>();
            foreach (string str in keys)
            {
                string[] parts = str.Split(':');

                // 如果字符串中包含冒号，则将第一部分和第二部分进行分组
                if (parts.Length >= 2)
                {
                    string group = $"{parts[0]}:{parts[1]}";
                    resultSet.Add(group);
                }
                // 如果字符串中不包含冒号，则直接进行分组
                else
                {
                    resultSet.Add(str);
                }
            }
            return resultSet.ToList();

        }



        private void VerifyRedisCacheEnable()
        {
            if (!EnableRedisCache)
            {
                throw new UserFriendlyException("后端程序未使用Redis缓存，无法对Redis进行监控，可切换使用Redis");
            }

        }

        [HttpGet("monitor-cache/key/{cacaheName}")]
        public List<string> GetKey(string cacaheName)
        {
            VerifyRedisCacheEnable();
            var output = RedisClient.Keys($"{cacaheName}:*").Select(x => x.RemovePreFix(cacaheName + ":"));
            return output.ToList();
        }

        //全部不为空
        [HttpGet("monitor-cache/value/{cacaheName}/{cacaheKey}")]
        public MonitorCacheGetListOutputDto GetValue(string cacaheName, string cacaheKey)
        {
            var value = RedisClient.HGet($"{cacaheName}:{cacaheKey}", "data");
            return new MonitorCacheGetListOutputDto() { CacheKey = cacaheKey, CacheName = cacaheName, CacheValue = value };
        }



        [HttpDelete("monitor-cache/key/{cacaheName}")]
        public bool DeleteKey(string cacaheName)
        {
            VerifyRedisCacheEnable();
            RedisClient.Del($"{cacaheName}:*");
            return true;
        }

        [HttpDelete("monitor-cache/value/{cacaheName}/{cacaheKey}")]
        public bool DeleteValue(string cacaheName, string cacaheKey)
        {
            RedisClient.Del($"{cacaheName}:{cacaheKey}");
            return true;
        }

        [HttpDelete("monitor-cache/clear")]
        public bool DeleteClear()
        {
            RedisClient.FlushDb();
            return true;
        }
    }


}
