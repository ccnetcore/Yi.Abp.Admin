using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Application.Services;
using Yi.Framework.Rbac.Application.Contracts.Dtos.MonitorCache;
using Yi.Framework.Rbac.Application.Contracts.IServices;

namespace Yi.Framework.Rbac.Application.Services
{
    public class MonitorCacheService : ApplicationService, IMonitorCacheService
    {
        private static List<MonitorCacheNameGetListOutputDto> monitorCacheNames => new List<MonitorCacheNameGetListOutputDto>()
        {
          new MonitorCacheNameGetListOutputDto{ CacheName="Yi:Login",Remark="登录验证码"},
             new MonitorCacheNameGetListOutputDto{ CacheName="Yi:User",Remark="用户信息"}
        };
        private Dictionary<string, string> monitorCacheNamesDic => monitorCacheNames.ToDictionary(x => x.CacheName, x => x.Remark);
        //private CSRedisClient _cacheClient;
        public MonitorCacheService()
        {
            //_cacheClient = redisCacheClient.Client;
        }
        //cacheKey value为空，只要name和备注

        public List<MonitorCacheNameGetListOutputDto> GetName()
        {
            //固定的
            return monitorCacheNames;
        }
        [HttpGet("key/{cacaheName}")]
        public List<string> GetKey(string cacaheName)
        {
            //var output = _cacheClient.Keys($"{cacaheName}:*");
            return new List<string>() { "1233124", "3124", "1231251", "12312412" };
        }

        //全部不为空
        [HttpGet("value/{cacaheName}/{cacaheKey}")]
        public MonitorCacheGetListOutputDto GetValue(string cacaheName, string cacaheKey)
        {
            //var value = _cacheClient.Get($"{cacaheName}:{cacaheKey}");
            return new MonitorCacheGetListOutputDto() { CacheKey = cacaheKey, CacheName = cacaheName, CacheValue = "ttt", Remark = monitorCacheNamesDic[cacaheName] };
        }
    }


}
