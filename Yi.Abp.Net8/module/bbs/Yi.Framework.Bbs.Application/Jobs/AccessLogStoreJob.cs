using FreeRedis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Quartz;
using Volo.Abp.BackgroundWorkers.Quartz;
using Volo.Abp.Caching;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities;
using Yi.Framework.Bbs.Domain.Entities;
using Yi.Framework.Bbs.Domain.Shared.Caches;
using Yi.Framework.Bbs.Domain.Shared.Enums;
using Yi.Framework.SqlSugarCore.Abstractions;

namespace Yi.Framework.Bbs.Application.Jobs;

public class AccessLogStoreJob : QuartzBackgroundWorkerBase
{
    private readonly ISqlSugarRepository<AccessLogAggregateRoot> _repository;

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

    public AccessLogStoreJob(ISqlSugarRepository<AccessLogAggregateRoot> repository)
    {
        _repository = repository;
        JobDetail = JobBuilder.Create<AccessLogStoreJob>().WithIdentity(nameof(AccessLogStoreJob))
            .Build();
        
        //每分钟执行一次
        Trigger = TriggerBuilder.Create().WithIdentity(nameof(AccessLogStoreJob))
            .WithCronSchedule("0 * * * * ?")
            .Build();
        
    
    }

    public override async Task Execute(IJobExecutionContext context)
    {
        if (EnableRedisCache)
        {
            //当天的访问量
            var number =
                await RedisClient.GetAsync<long>($"{CacheKeyPrefix}:{AccessLogCacheConst.Key}:{DateTime.Now.Date}");


            var entity = await _repository._DbQueryable.Where(x => x.AccessLogType == AccessLogTypeEnum.Request)
                .Where(x => x.CreationTime.Date == DateTime.Today)
                .FirstAsync();


            if (entity is not null)
            {
                entity.Number = number+1;
                await _repository.UpdateAsync(entity);
            }
            else
            {
                await _repository.InsertAsync((new AccessLogAggregateRoot() { Number = number,AccessLogType = AccessLogTypeEnum.Request}));
            }

            //删除前一天的缓存
            await RedisClient.DelAsync($"{CacheKeyPrefix}:{AccessLogCacheConst.Key}:{DateTime.Now.Date.AddDays(-1)}");
        }
    }
}