using FreeRedis;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Volo.Abp.Caching;

namespace Yi.Framework.Caching.FreeRedis
{
    /// <summary>
    /// 此模块得益于FreeRedis作者支持IDistributedCache，使用湿滑
    /// </summary>
    [DependsOn(typeof(AbpCachingModule))]
    public class YiFrameworkCachingFreeRedisModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {

            var configuration = context.Services.GetConfiguration();

            var redisEnabled = configuration["Redis:IsEnabled"];
            if (redisEnabled.IsNullOrEmpty() || bool.Parse(redisEnabled))
            {
                var redisConfiguration = configuration["Redis:Configuration"];
                RedisClient redisClient = new RedisClient(redisConfiguration);

                context.Services.AddSingleton<IRedisClient>(redisClient);
                context.Services.Replace(ServiceDescriptor.Singleton<IDistributedCache>(new
                     DistributedCache(redisClient)));
            }
        }
    }
}
