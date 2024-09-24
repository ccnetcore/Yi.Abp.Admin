using FreeRedis;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Volo.Abp.Caching;
using Volo.Abp.DependencyInjection;
using Yi.Framework.Bbs.Domain.Shared.Caches;

namespace Yi.Framework.Bbs.Application.Extensions;

/// <summary>
/// 访问日志中间件
/// 并发最高，采用缓存，默认10分钟才会真正操作一次数据库
/// 需考虑一致性问题，又不能上锁影响性能
/// </summary>
public class AccessLogMiddleware : IMiddleware, ITransientDependency
{
    /// <summary>
    /// 缓存前缀
    /// </summary>
    private string CacheKeyPrefix => LazyServiceProvider.LazyGetRequiredService<IOptions<AbpDistributedCacheOptions>>()
        .Value.KeyPrefix;

    /// <summary>
    /// 使用懒加载防止报错
    /// </summary>
    private IRedisClient RedisClient => LazyServiceProvider.LazyGetRequiredService<IRedisClient>();

    /// <summary>
    /// 属性注入
    /// </summary>
    public IAbpLazyServiceProvider LazyServiceProvider { get; set; }

    private bool EnableRedisCache
    {
        get
        {
            var redisEnabled = LazyServiceProvider.LazyGetRequiredService<IConfiguration>()["Redis:IsEnabled"];
            return redisEnabled.IsNullOrEmpty() || bool.Parse(redisEnabled);
        }
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        await next(context);
        if (EnableRedisCache)
        {
            await RedisClient.IncrByAsync($"{CacheKeyPrefix}:{AccessLogCacheConst.Key}:{DateTime.Now.Date}", 1);
        }
    }
}