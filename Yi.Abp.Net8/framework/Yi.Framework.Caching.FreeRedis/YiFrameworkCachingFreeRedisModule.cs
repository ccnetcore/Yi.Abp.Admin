using FreeRedis;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Volo.Abp.Caching;

namespace Yi.Framework.Caching.FreeRedis
{
    /// <summary>
    /// FreeRedis缓存模块
    /// 提供基于FreeRedis的分布式缓存实现
    /// </summary>
    [DependsOn(typeof(AbpCachingModule))]
    public class YiFrameworkCachingFreeRedisModule : AbpModule
    {
        private const string RedisEnabledKey = "Redis:IsEnabled";
        private const string RedisConfigurationKey = "Redis:Configuration";

        /// <summary>
        /// 配置服务
        /// </summary>
        /// <param name="context">服务配置上下文</param>
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var configuration = context.Services.GetConfiguration();

            // 检查Redis是否启用
            if (!IsRedisEnabled(configuration))
            {
                return;
            }

            // 注册Redis服务
            RegisterRedisServices(context, configuration);
        }

        /// <summary>
        /// 检查Redis是否启用
        /// </summary>
        /// <param name="configuration">配置</param>
        /// <returns>是否启用Redis</returns>
        private static bool IsRedisEnabled(IConfiguration configuration)
        {
            var redisEnabled = configuration[RedisEnabledKey];
            return redisEnabled.IsNullOrEmpty() || bool.Parse(redisEnabled);
        }

        /// <summary>
        /// 注册Redis相关服务
        /// </summary>
        /// <param name="context">服务配置上下文</param>
        /// <param name="configuration">配置</param>
        private static void RegisterRedisServices(ServiceConfigurationContext context, IConfiguration configuration)
        {
            var redisConfiguration = configuration[RedisConfigurationKey];
            var redisClient = new RedisClient(redisConfiguration);

            context.Services.AddSingleton<IRedisClient>(redisClient);
            context.Services.Replace(ServiceDescriptor.Singleton<IDistributedCache>(
                new DistributedCache(redisClient)));
        }
    }
}
