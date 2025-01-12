using FreeRedis;
using Hangfire;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Volo.Abp.BackgroundWorkers.Hangfire;
using Volo.Abp.Caching;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities;
using Volo.Abp.EventBus.Local;
using Yi.Framework.Bbs.Domain.Entities;
using Yi.Framework.Bbs.Domain.Shared.Caches;
using Yi.Framework.Bbs.Domain.Shared.Enums;
using Yi.Framework.Bbs.Domain.Shared.Etos;
using Yi.Framework.SqlSugarCore.Abstractions;

namespace Yi.Framework.Bbs.Application.Jobs;

public class AccessLogCacheJob : HangfireBackgroundWorkerBase
{
    private readonly ILocalEventBus _localEventBus;

    public AccessLogCacheJob(ILocalEventBus localEventBus)
    {
        _localEventBus = localEventBus;
        RecurringJobId = "访问日志写入缓存";
        //每10秒执行一次，将本地缓存转入redis，防止丢数据
        CronExpression = "*/10 * * * * *";
        //
        // JobDetail = JobBuilder.Create<AccessLogCacheJob>().WithIdentity(nameof(AccessLogCacheJob))
        //     .Build();

        //每10秒执行一次，将本地缓存转入redis，防止丢数据
        // Trigger = TriggerBuilder.Create().WithIdentity(nameof(AccessLogCacheJob))
        //     .WithSimpleSchedule((schedule) => { schedule.WithInterval(TimeSpan.FromSeconds(10)).RepeatForever();; })
        //     .Build();
    }
    
    public override async Task DoWorkAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        await _localEventBus.PublishAsync(new AccessLogResetArgs());
    }
}